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
                    OnProgress = delegate(string output)
                    {
                        Console.WriteLine("Fetch.OnProgress : " + output);
                        return true;
                    }
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

        public void Update(string branchName)
        {
            var branch = _repo.Branches[branchName];

            Console.WriteLine("\n\t updating : " + branch);

            // remote not tracked
            if (branch.IsRemote && _repo.Branches.All(x => x.TrackedBranch != branch))
            {
                Console.WriteLine("\t create and checkout : " + branch);
                var newBranch = _repo.CreateBranch(branchName, branch.Tip);
                _repo.Branches.Update(newBranch, b => b.TrackedBranch = branch.CanonicalName);
            }
            else
            {
                Console.WriteLine("\t checkout and pull : " + branch);
                _repo.Checkout(branchName);
                _repo.Network.Pull(
                    new Signature("sd", "sd", DateTimeOffset.Now),
                    new PullOptions
                    {
                        MergeOptions = new MergeOptions
                        {
                            FastForwardStrategy = FastForwardStrategy.FastForwardOnly
                        }
                    });
            }

            branch = _repo.Branches[branchName];

            if (branch.Tip != null)
            {
                Console.WriteLine("\t tip @ {0} - {1} ", branch.Tip.Committer.When, branch.Tip.Message);
            }
        }

        private static Repository GetRepository(string repoUrl, string workingDir)
        {
            if (Directory.EnumerateFileSystemEntries(workingDir).Any() == false)
            {
                Repository.Clone(repoUrl, workingDir);
            }

            return new Repository(workingDir);
        }
    }
}
