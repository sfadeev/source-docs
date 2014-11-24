using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace SourceDocs.Core.Tests
{
    public class GitRepository
    {
        private readonly Repository _repo;

        // todo: set specified timeout to delayed generate
        public GitRepository(string repoUrl)
        {
            Console.WriteLine("\nRepo : " + repoUrl);

            _repo = EnsureRepository(repoUrl, "./repos/");
        }

        public IEnumerable<string> GetChangedBranches()
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

            foreach (var branch in _repo.Branches)
            {
                // local with commits behind
                if (branch.IsTracking && branch.TrackingDetails != null && branch.TrackingDetails.BehindBy > 0)
                {
                    Console.WriteLine("\t local : {0}, {1} behind", branch, branch.TrackingDetails.BehindBy);

                    yield return branch.Name;
                }
                // remote not tracked
                else if (branch.IsRemote && _repo.Branches.All(x => x.TrackedBranch != branch))
                {
                    Console.WriteLine("\t remote : {0}", branch);

                    yield return branch.Name;
                }
            }
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
        }

        private static Repository EnsureRepository(string repoUrl, string workingDir)
        {
            var repositoryDir = GetRepositoryDirectory(repoUrl, workingDir);

            if (repositoryDir.Exists == false)
            {
                repositoryDir.Create();

                Repository.Clone(repoUrl, repositoryDir.FullName);
            }

            return new Repository(repositoryDir.FullName);
        }

        private static DirectoryInfo GetRepositoryDirectory(string repoUrl, string workingDir)
        {
            var repositoryDirName = Path.GetInvalidFileNameChars()
                .Aggregate(repoUrl, (current, invalidFileNameChar) => current.Replace(invalidFileNameChar, '_'));

            return new DirectoryInfo(Path.Combine(workingDir, repositoryDirName, "repo"));
        }
    }
}
