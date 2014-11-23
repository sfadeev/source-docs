using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using SourceDocs.Core.Generation;

namespace SourceDocs.Core.Tests
{
    public class RepositoryTask
    {
        public Repository Repository { get; set; }

        public Branch Branch { get; set; }
    }

    public class Workflow
    {
        public Workflow()
        {
            IGenerator generator = new GitGenerator();

            if (generator.ShouldGenerate())
            {
                // set specified timeout to delayed generate
            }
        }

        public IEnumerable<RepositoryTask> GetTasks(string repoUrl)
        {
            Console.WriteLine();
            Console.WriteLine(repoUrl);

            var repo = EnsureRepository(repoUrl, "./repos/");

            foreach (var branch in repo.Branches)
            {
                Console.WriteLine("Branch : " + branch);

                if (ShouldCheckout(repo, branch))
                {
                    yield return new RepositoryTask { Repository = repo, Branch = branch };
                }
            }
        }

        private static bool ShouldCheckout(Repository repo, Branch branch)
        {
            return
                // local with commits behind
                (branch.IsTracking && branch.TrackingDetails != null && branch.TrackingDetails.BehindBy > 0) ||
                // remote not tracked
                (branch.IsRemote && repo.Branches.All(x => x.TrackedBranch != branch));
        }

        public void Execute(RepositoryTask task)
        {
            var repo = task.Repository;
            var branch = task.Branch;

            // remote not tracked
            if (branch.IsRemote && repo.Branches.All(x => x.TrackedBranch != branch))
            {
                Console.WriteLine("Create and checkout : " + branch);
                var newBranch = repo.CreateBranch(branch.Name.Replace("origin/", string.Empty), branch.Tip);
                repo.Branches.Update(newBranch, b => b.TrackedBranch = branch.CanonicalName);
            }
            else
            {
                Console.WriteLine("Checkout : " + branch);
                task.Branch = repo.Checkout(branch.Tip);
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
