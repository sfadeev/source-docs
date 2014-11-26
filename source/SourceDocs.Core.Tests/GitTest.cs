using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SourceDocs.Core.Generation;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class GitTest
    {
        // [TestCase("https://github.com/sfadeev/renocco.git")]
        [TestCase("c:\\data\\projects\\temp\\SomeRepo")]
        public void WorkflowTest(string repoUrl)
        {
            var repo = new GitRepository(repoUrl, GetWorkingDir("./repos/", repoUrl, "repo"));

            foreach (var branchName in repo.GetBranches())
            {
                repo.Update(branchName);
            }

            while (true)
            {
                foreach (var branchName in repo.GetBranches(changedOnly: true))
                {
                    repo.Update(branchName);
                }

                Thread.Sleep(5000);
            }
        }

        public static string GetWorkingDir(string workingRoot, string repoUrl, params string[] repoPaths)
        {
            Func<string, string> fixDirName = dirName =>
                Path.GetInvalidFileNameChars().Aggregate(dirName, (current, c) => current.Replace(c, '_'));

            var workingDir = Path.Combine(new[] { workingRoot, fixDirName(repoUrl) }.Concat(repoPaths).ToArray());

            if (Directory.Exists(workingDir) == false)
            {
                Directory.CreateDirectory(workingDir);
            }

            return workingDir;
        }
    }
}
