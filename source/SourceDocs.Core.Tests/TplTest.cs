using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class TplTest
    {
        [Test]
        public void TplDataflow()
        {
            // blocks
            var generateWords = new TransformBlock<int, string>(count =>
            {
                Console.WriteLine("generating " + count + " words (thread id: " + Thread.CurrentThread.ManagedThreadId + ")");

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
                Console.WriteLine("splitting " + s + " (thread id: " + Thread.CurrentThread.ManagedThreadId + ")\n\t");

                return s.Split(new[] { ' ' });
            });

            var sortWords = new TransformBlock<string[], string[]>(strings =>
            {
                Console.WriteLine("sorting (thread id: " + Thread.CurrentThread.ManagedThreadId + ")");

                foreach (var s in strings)
                {
                    Console.WriteLine("\t" + s);
                }
                
                return strings.OrderBy(s => s.Length).ToArray();
            });

            var printWords = new ActionBlock<string[]>(strings =>
            {
                Console.WriteLine("printing (thread id: " + Thread.CurrentThread.ManagedThreadId + ")");

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

        [Test]
        public void TplDataflowSuccessAndError()
        {
            Action<Task> continuationAction = task =>
            {
                Console.WriteLine("Continuation Action");
                Console.WriteLine("===================");
                Console.WriteLine("task.Id              : " + task.Id);
                Console.WriteLine("task.IsCanceled      : " + task.IsCanceled);
                Console.WriteLine("task.IsCompleted     : " + task.IsCompleted);
                Console.WriteLine("task.IsFaulted       : " + task.IsFaulted);
                Console.WriteLine("task.Status          : " + task.Status);
                Console.WriteLine("task.CreationOptions : " + task.CreationOptions);
                Console.WriteLine("task.Exception       : " + task.Exception);
            };

            var block = new ActionBlock<int>(no =>
            {
                if (no == 13) throw new InvalidOperationException("Error # " + no);

                Console.WriteLine("# " + no);
            });

            for (var i = 0; i < 15; i++)
            {
                block.Post(i);
            }

            block.Complete();
            block.Completion.ContinueWith(continuationAction);

            try
            {
                block.Completion.Wait();
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("AggregateException");
                Console.WriteLine("==================");

                ae.Handle(e =>
                {
                    Console.WriteLine("\nHandled: " + e);
                    Console.WriteLine();
                    return true;
                });
            }
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