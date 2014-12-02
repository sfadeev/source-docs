using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace SourceDocs.Core.Generation
{
    public class GitRepository
    {
        private readonly Repository _repo;

        // todo: set specified timeout to delayed generate
        public GitRepository(string repoUrl, string workingDir)
        {
            Console.WriteLine("\nRepo : " + repoUrl);

            _repo = GetRepository(repoUrl, workingDir);
        }

        public void Fetch()
        {
            foreach (var remote in _repo.Network.Remotes)
            {
                Console.WriteLine("\n Fetching from : " + remote.Name);

                _repo.Fetch(remote.Name, new FetchOptions
                {
                    OnProgress = OnProgress, OnTransferProgress = OnTransferProgress
                });
            }
        }

        public string[] GetBranches(bool changedOnly = false)
        {
            Fetch();

            var result = new List<string>();

            foreach (var branch in _repo.Branches)
            {
                if (changedOnly)
                {
                    if (branch.IsTracking && branch.TrackingDetails != null && branch.TrackingDetails.BehindBy > 0)
                    {
                        Console.WriteLine("\t changed local : {0}, {1} behind", branch, branch.TrackingDetails.BehindBy);

                        result.Add(branch.Name);
                    }
                    else if (branch.IsRemote && _repo.Branches.All(x => x.TrackedBranch != branch))
                    {
                        Console.WriteLine("\t not tracked remote : {0}", branch);

                        result.Add(branch.Name);
                    }
                }
                else
                {
                    // all local and remote not tracked
                    if (branch.IsTracking || (branch.IsRemote && _repo.Branches.All(x => x.TrackedBranch != branch)))
                    {
                        Console.WriteLine("\t branch : {0}", branch);

                        result.Add(branch.Name);
                    }
                }
            }

            return result.ToArray();
        }

        public Commit Update(string branchName)
        {
            var branch = _repo.Branches[branchName];

            Console.WriteLine("\n\t updating : " + branch);

            // remote not tracked
            if (branch.IsRemote && _repo.Branches.All(x => x.TrackedBranch != branch))
            {
                Console.WriteLine("\t create and checkout : " + branch);

                var trackedBranch = branch;
                var localBranch = _repo.CreateBranch(branchName, trackedBranch.Tip);

                _repo.Branches.Update(localBranch, b => b.TrackedBranch = trackedBranch.CanonicalName);
            }
            else if (branch.IsTracking && branch.TrackingDetails != null && branch.TrackingDetails.BehindBy > 0)
            {
                Checkout(branchName);

                Console.WriteLine("\t pull : " + branch);

                var mergeResult = _repo.Network.Pull(new Signature("sd", "sd", DateTimeOffset.Now), new PullOptions
                {
                    MergeOptions = new MergeOptions
                    {
                        FastForwardStrategy = FastForwardStrategy.FastForwardOnly,
                        OnCheckoutProgress = OnCheckoutProgress
                    },
                    FetchOptions = new FetchOptions
                    {
                        OnProgress = OnProgress, OnTransferProgress = OnTransferProgress
                    }
                });

                Console.WriteLine("merge result : {0}, {1}", mergeResult.Status, mergeResult.Commit);
            }

            branch = _repo.Branches[branchName];

            if (branch.Tip != null)
            {
                Console.WriteLine("\t tip @ {0} - {1} ", branch.Tip.Committer.When, branch.Tip.Message);
            }

            return branch.Tip;
        }

        public Branch Checkout(string branchName)
        {
            Console.WriteLine("\t checkout : " + branchName);
            
            return _repo.Checkout(branchName, new CheckoutOptions
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
            Console.WriteLine("OnCheckoutProgress : [{0}/{1}] {2}", steps, totalSteps, path);
        }

        private static bool OnTransferProgress(TransferProgress progress)
        {
            Console.WriteLine("OnTransferProgress : [{0}/{1}/{2}] {3}", progress.IndexedObjects, progress.ReceivedObjects, progress.TotalObjects, progress.ReceivedBytes);
            return true;
        }
    }
}
