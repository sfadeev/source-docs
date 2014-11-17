using System;
using LibGit2Sharp;
using NUnit.Framework;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test()
        {
            var clonedRepoPath = Repository.Clone("https://github.com/sfadeev/source-docs.git", "./temp-repo/");

            using (var repo = new Repository(clonedRepoPath))
            {
                foreach (var item in repo.RetrieveStatus())
                {
                    Console.WriteLine(item.FilePath);
                }
            }
        }
    }
}
