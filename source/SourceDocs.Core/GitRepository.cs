using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using SourceDocs.Core.Models;

namespace SourceDocs.Core
{
    public class GitRepository : IRepository, IDisposable
    {
        public class Settings
        {
            /// <summary>
            /// The URL of the remote Git repository.
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// Repository configuration file path.
            /// </summary>
            public string ConfigFile { get; set; }

            /// <summary>
            /// Working directory of repository.
            /// </summary>
            public string WorkingDirectory { get; set; }

            public string BaseDirectory { get; set; }

            /// <summary>
            /// Patterns of branches to be monitored on changes in format +:* (to include branches) or -:*  (to exclude branches).
            /// </summary>
            [DefaultValue(new[] { "+:refs/heads/*" })]
            public string[] Branches { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }
        }

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Settings _settings;

        private readonly Repository _git;

        // todo: set specified timeout to delayed generate
        public GitRepository(Settings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            _settings = settings;

            _git = GetRepository();

            /*foreach (var branch in _git.Branches)
            {
                Console.WriteLine("\t[{0}] [{1}] {2} ({3}) {4} {5}",
                    branch.IsRemote ? "R" : " ", branch.IsTracking ? "T" : " ",
                    branch.Name, branch.CanonicalName,
                    branch.Remote != null ? "\n\t +  \t " + branch.Remote.Name + " (" + branch.Remote.Url + ")" : string.Empty,
                    branch.TrackedBranch != null ? "\n\t     + \t " + branch.TrackedBranch.Name : string.Empty);
            }*/
        }

        public string Url
        {
            get { return _settings.Url; }
        }

        public IList<Node> UpdateNodes(IEnumerable<Node> nodes)
        {
            Fetch();

            var branches = _git.Branches.ToList();

            // exclude local branches removed on remote
            var result = (nodes ?? Enumerable.Empty<Node>())
                .Where(node => branches.Any(branch => branch.IsRemote && branch.CanonicalName == node.RemoteName)).ToList();

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

            var branches = _git.Branches.ToList();

            var remoteBranch = branches.FirstOrDefault(x => x.IsRemote && x.CanonicalName == node.RemoteName);
            var localBranch = branches.FirstOrDefault(x => x.IsTracking && x.TrackedBranch.CanonicalName == node.RemoteName);

            // remote not tracked
            if (remoteBranch != null && localBranch == null)
            {
                if (node.Name == null)
                    node.Name = GenerateLocalBranchName(remoteBranch);

                Console.WriteLine("Checkout {0} from {1} ({2})", node.Name, remoteBranch.Name, remoteBranch.CanonicalName);

                localBranch = _git.Branches.Update(
                    _git.CreateBranch(node.Name, remoteBranch.Tip),
                    b => b.TrackedBranch = remoteBranch.CanonicalName);

                node.Updated = GetDateUpdated(localBranch);

                return true;
            }

            if (localBranch != null && localBranch.TrackingDetails.BehindBy > 0)
            {
                localBranch = Checkout(localBranch);

                Console.WriteLine("Pull : " + localBranch);

                var mergeResult = _git.Network.Pull(new Signature("sd", "sd", DateTimeOffset.Now), new PullOptions
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
            foreach (var remote in _git.Network.Remotes)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Fetching from {0}@{1}", remote.Name, _settings.Url);
                }
                
                // todo: Branches removed on origin is not removed in local repository
                // Remote prune #2700 - status is Open as of 12/07/2014
                // https://github.com/libgit2/libgit2/pull/2700

                var fetchOptions = new FetchOptions
                {
                    OnUpdateTips = (name, id, newId) =>
                    {
                        Console.WriteLine("\nFetch.OnUpdateTips : {0},  {1},  {2}", name, id, newId);
                        return true;
                    },
                    OnProgress = OnProgress,
                    OnTransferProgress = OnTransferProgress
                };

                _git.Network.Fetch(remote, fetchOptions);
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
            
            return _git.Checkout(branch, new CheckoutOptions
            {
                OnCheckoutProgress = OnCheckoutProgress
            });
        }

        private Repository GetRepository()
        {
            if (Directory.EnumerateFileSystemEntries(_settings.WorkingDirectory).Any() == false)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Cloning git repository {0} in {1}", _settings.Url, _settings.WorkingDirectory);
                }

                var cloneDirectory = Repository.Clone(_settings.Url, _settings.WorkingDirectory, new CloneOptions
                {
                    OnCheckoutProgress = OnCheckoutProgress,
                    OnTransferProgress = OnTransferProgress
                });

                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("Cloned git repository {0} in {1} ({2})", _settings.Url, _settings.WorkingDirectory, cloneDirectory);
                }
            }

            var repository = new Repository(_settings.WorkingDirectory);

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Initialized git repository {0} in {1}", _settings.Url, _settings.WorkingDirectory);
            }

            return repository;
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
            _git.Dispose();
        }
    }
}
