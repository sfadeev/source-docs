using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class GitTest
    {
        [Test, Ignore]
        public void WorkflowTest()
        {
            // var repo = new GitRepository("https://github.com/sfadeev/renocco.git");
            var repo = new GitRepository(@"C:\Data\Projects\temp\SomeRepo");

            while (true)
            {
                var branchNames = repo.GetChangedBranches().ToArray();

                foreach (var branchName in branchNames)
                {
                    repo.Update(branchName);
                }

                Thread.Sleep(3000);
            }
        }
    }
}
