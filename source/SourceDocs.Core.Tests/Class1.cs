using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;
using LibGit2Sharp;
using NUnit.Framework;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void CloneRepo()
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

        [Test]
        public void TplDataflow()
        {
            // blocks
            var generateWords = new TransformBlock<int, string>(count =>
            {
                Console.WriteLine("generating " + count + " words");

                var result = new StringBuilder();

                var random = new Random();

                for (var i = 0; i < count; i++)
                {
                    var word = GenerateWord(random, random.Next(10) + 2);

                    if (result.Length != 0) result.Append(' ');

                    result.Append(word);
                }

                return result.ToString();
            });

            var splitWords = new TransformBlock<string, string[]>(s =>
            {
                Console.WriteLine("splitting \n\t" + s);

                return s.Split(new[] { ' ' });
            });

            var sortWords = new TransformBlock<string[], string[]>(strings =>
            {
                Console.WriteLine("sorting");

                foreach (var s in strings)
                {
                    Console.WriteLine("\t" + s);
                }
                
                return strings.OrderBy(s => s.Length).ToArray();
            });

            var printWords = new ActionBlock<string[]>(strings =>
            {
                Console.WriteLine("printing ");

                foreach (var s in strings)
                {
                    Console.WriteLine("\t" + s);
                }
            });

            // connect
            generateWords.Completion.ContinueWith(t => splitWords.Complete());
            generateWords.LinkTo(splitWords);

            splitWords.Completion.ContinueWith(t => sortWords.Complete());
            splitWords.LinkTo(sortWords);

            sortWords.Completion.ContinueWith(t => printWords.Complete());
            sortWords.LinkTo(printWords);

            // run
            generateWords.Post(5);
            generateWords.Complete();
            printWords.Completion.Wait();
        }

        private static string GenerateWord(Random random, int length)
        {
            string[] vowels = { "a", "e", "i", "o", "u" };
            string[] consonants =
            {
                "b", "c", "d", "f", "g", "h", "j",
                "k", "l", "m", "n", "p", "q", "r",
                "s", "t", "v", "w", "x", "y", "z"
            };

            var word = string.Empty; 
            for (var i = 0; i < length; i += 2)
            {
                word += consonants[random.Next(0, consonants.Length - 1)] + vowels[random.Next(0, vowels.Length - 1)];
            }
            return word;
        }
    }
}
