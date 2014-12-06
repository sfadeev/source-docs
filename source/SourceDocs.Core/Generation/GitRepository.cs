using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace SourceDocs.Core.Generation
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Node> UpdateNodes(IEnumerable<Node> nodes);

        bool UpdateNode(Node node);
    }

    public class GitRepository : IRepository
    {
        private readonly Repository _repo;

        // todo: set specified timeout to delayed generate
        public GitRepository(string repoUrl, string workingDir)
        {
            Console.WriteLine("\nRepo : " + repoUrl);

            _repo = GetRepository(repoUrl, workingDir);

            /*foreach (var branch in _repo.Branches)
            {
                Console.WriteLine("\t[{0}] [{1}] {2} ({3}) {4} {5}",
                    branch.IsRemote ? "R" : " ", branch.IsTracking ? "T" : " ",
                    branch.Name, branch.CanonicalName,
                    branch.Remote != null ? "\n\t +  \t " + branch.Remote.Name + " (" + branch.Remote.Url + ")" : string.Empty,
                    branch.TrackedBranch != null ? "\n\t     + \t " + branch.TrackedBranch.Name : string.Empty);
            }*/
        }

        public IEnumerable<Node> UpdateNodes(IEnumerable<Node> nodes)
        {
            Fetch();

            var branches = _repo.Branches.ToList();

            // exclude local branches removed on remote
            var result = (nodes ?? Enumerable.Empty<Node>())
                .Where(x => branches.Any(b => b.IsRemote && b.CanonicalName == x.RemoteName)).ToList();

            foreach (var branch in branches)
            {
                if (branch.IsRemote)
                {
                    var resultNode = result.FirstOrDefault(x => x.RemoteName == branch.CanonicalName);

                    if (resultNode == null)
                    {
                        result.Add(resultNode = new Node
                        {
                            RemoteName = branch.CanonicalName,
                        });
                    }

                    if (resultNode.Name == null)
                        resultNode.Name = GenerateLocalBranchName(branch);

                    resultNode.Updated = GetDateUpdated(branch);
                }
            }

            return result;
        }

        public bool UpdateNode(Node node)
        {
            if (node == null) throw new ArgumentNullException("node");

            var branches = _repo.Branches.ToList();

            var remoteBranch = branches.FirstOrDefault(x => x.IsRemote && x.CanonicalName == node.RemoteName);
            var localBranch = branches.FirstOrDefault(x => x.IsTracking && x.TrackedBranch.CanonicalName == node.RemoteName);

            // remote not tracked
            if (remoteBranch != null && localBranch == null)
            {
                if (node.Name == null)
                    node.Name = GenerateLocalBranchName(remoteBranch);

                Console.WriteLine("Create and checkout {0} from {1} ({2})", node.Name, remoteBranch.Name,
                    remoteBranch.CanonicalName);

                localBranch = _repo.Branches.Update(
                    _repo.CreateBranch(node.Name, remoteBranch.Tip),
                    b => b.TrackedBranch = remoteBranch.CanonicalName);

                node.Updated = GetDateUpdated(localBranch);

                return true;
            }

            if (localBranch != null && localBranch.TrackingDetails.BehindBy > 0)
            {
                localBranch = Checkout(localBranch);

                Console.WriteLine("Pull : " + localBranch);

                var mergeResult = _repo.Network.Pull(new Signature("sd", "sd", DateTimeOffset.Now), new PullOptions
                {
                    MergeOptions = new MergeOptions
                    {
                        FastForwardStrategy = FastForwardStrategy.FastForwardOnly,
                        OnCheckoutProgress = OnCheckoutProgress
                    },
                    FetchOptions = new FetchOptions
                    {
                        OnUpdateTips = (name, id, newId) =>
                        {
                            Console.WriteLine("Fetch.OnUpdateTips : {0},  {1},  {2}", name, id, newId);
                            return true;
                        },
                        OnProgress = OnProgress,
                        OnTransferProgress = OnTransferProgress
                    }
                });

                Console.WriteLine("merge result : {0}, {1}", mergeResult.Status, mergeResult.Commit);

                node.Updated = GetDateUpdated(remoteBranch);

                return true;
            }

            return false;
        }

        private void Fetch()
        {
            foreach (var remote in _repo.Network.Remotes)
            {
                // Console.WriteLine("Fetching from : " + remote.Name);

                // Removed on origin is not removed in local repo
                // Remote prune #2700 - status is Open as of 12/07/2014
                // https://github.com/libgit2/libgit2/pull/2700

                var fetchOptions = new FetchOptions
                {
                    OnUpdateTips = (name, id, newId) =>
                    {
                        Console.WriteLine("Fetch.OnUpdateTips : {0},  {1},  {2}", name, id, newId);
                        return true;
                    },
                    OnProgress = OnProgress,
                    OnTransferProgress = OnTransferProgress
                };

                _repo.Network.Fetch(remote, fetchOptions);
            }
        }

        private static string GenerateLocalBranchName(Branch branch)
        {
            return branch.Name.StartsWith(branch.Remote.Name + "/")
                ? branch.Name.Substring(branch.Remote.Name.Length + 1)
                : branch.Name;
        }

        private static DateTimeOffset GetDateUpdated(Branch branch)
        {
            return branch.Tip.Author.When;
        }

        private Branch Checkout(Branch branch)
        {
            Console.WriteLine("Checkout : " + branch);
            
            return _repo.Checkout(branch, new CheckoutOptions
            {
                OnCheckoutProgress = OnCheckoutProgress
            });
        }

        private static Repository GetRepository(string repoUrl, string workingDir)
        {
            if (Directory.EnumerateFileSystemEntries(workingDir).Any() == false)
            {
                Repository.Clone(repoUrl, workingDir, new CloneOptions
                {
                    // Checkout = false,
                    // IsBare = true,
                    OnCheckoutProgress = OnCheckoutProgress,
                    OnTransferProgress = OnTransferProgress
                });
            }

            return new Repository(workingDir);
        }

        private static bool OnProgress(string serverProgressOutput)
        {
            Console.WriteLine("OnProgress : {0}", serverProgressOutput);
            return true;
        }

        private static void OnCheckoutProgress(string path, int steps, int totalSteps)
        {
            // Console.WriteLine("OnCheckoutProgress : [{0}/{1}] {2}", steps, totalSteps, path);
        }

        private static bool OnTransferProgress(TransferProgress progress)
        {
            // Console.WriteLine("OnTransferProgress : [{0}/{1}/{2}] {3}", progress.IndexedObjects, progress.ReceivedObjects, progress.TotalObjects, progress.ReceivedBytes);
            return true;
        }

        public void Dispose()
        {
            _repo.Dispose();
        }
    }
}
