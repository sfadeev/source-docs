using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class GitTest
    {
        [TestCase("https://github.com/sfadeev/renocco.git")]
        [TestCase("https://github.com/sfadeev/source-docs.git")]
        [TestCase("git://github.com/progit/progit2.git")]
        [TestCase("https://hg@bitbucket.org/tortoisehg/thg")]
        [TestCase("http://repo.or.cz/sqlgg.git")]
        [TestCase("c:\\data\\projects\\temp\\SomeRepo")]
        public void WorkflowTest(string repoUrl)
        {
            var repoDir = GetWorkingDir("./repos/", repoUrl, "repo");
            var configFile = Path.Combine(GetWorkingDir("./repos/", repoUrl), "config.json");

            var gitSettings = new GitRepository.Settings
            {
                Url = repoUrl,
                WorkingDirectory = repoDir
            };

            using (IRepository repo = new GitRepository(gitSettings))
            {
                var config = File.Exists(configFile)
                    ? JsonConvert.DeserializeObject<Repo>(File.ReadAllText(configFile))
                    : new Repo();

                Action writeConfig = () =>
                {
                    var serializeObject = JsonConvert.SerializeObject(config, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });

                    File.WriteAllText(configFile, serializeObject);
                };

                while (true)
                {
                    config.Nodes = repo.UpdateNodes(config.Nodes).ToList();

                    writeConfig();

                    Thread.Sleep(1000);

                    Console.Write("."); // for pretty test ;)

                    Node node;
                    while ((node = config.Nodes.FirstOrDefault(x => x.Generated == null || x.Updated > x.Generated)) != null)
                    {
                        repo.UpdateNode(node);

                        // generate docs
                        var tempDir = GetWorkingDir("./repos/", repoUrl, "temp");
                        Console.WriteLine("Generating docs for {0} in {1}", node.Name, tempDir);
                        EmptyDirectory(tempDir);

                        TransformDirectory(repoDir, tempDir, new TransformOptions
                        {
                            ExcludeDirectories = new[] { "\\docs", "\\bin", "\\obj", "\\packages", "\\.nuget", ".git", "\\.svn" },
                            FileTransformers = new Dictionary<string, IFileTransformer>
                            {
                                { ".md", new MarkdownFileTransformer() }
                            }
                        });

                        var outDir = GetWorkingDir("./repos/", repoUrl, "docs", node.Name);
                        Console.WriteLine("Copying docs for {0} to {1}", node.Name, outDir);
                        EmptyDirectory(outDir);
                        CopyDirectory(tempDir, outDir); // ready docs

                        EmptyDirectory(tempDir);

                        node.Generated = node.Updated;

                        writeConfig();

                        Thread.Sleep(1000);
                    }
                }
            }
        }

        public static string GetWorkingDir(string workingRoot, string repoUrl, params string[] repoPaths)
        {
            Func<string, string> fixDirName = dirName =>
                Path.GetInvalidFileNameChars().Aggregate(dirName, (current, c) => current.Replace(c, '_'));

            var workingDir = Path.Combine(new[] { workingRoot, fixDirName(repoUrl) }.Concat(repoPaths.Select(fixDirName)).ToArray());

            if (Directory.Exists(workingDir) == false)
            {
                Directory.CreateDirectory(workingDir);
            }

            return workingDir;
        }

        public static void EmptyDirectory(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in directoryInfo.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        public class TransformOptions
        {
            public string[] ExcludeDirectories { get; set; }

            public IDictionary<string, IFileTransformer> FileTransformers { get; set; }
        }

        public interface IFileTransformer
        {
            string Transform(string input);
        }

        public class MarkdownFileTransformer : IFileTransformer
        {
            public string Transform(string input)
            {
                var md = new MarkdownDeep.Markdown
                {
                    ExtraMode = true,
                    SafeMode = false
                };

                return md.Transform(input);
            }
        }

        public static void TransformDirectory(string from, string to, TransformOptions transformOptions)
        {
            if (transformOptions == null) throw new ArgumentNullException("transformOptions");

            // var foldersToExclude = new[] { "\\docs", "\\bin", "\\obj", "\\packages", "\\.nuget", ".git", "\\.svn" };

            var directoryInfo = new DirectoryInfo(from);
            foreach (var fileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                var relativePath = GetRelativePath(directoryInfo.FullName, fileSystemInfo.FullName);
                if (transformOptions.ExcludeDirectories.Any(s => relativePath.Contains(s)))
                {
                    continue;
                }

                var destFileName = Path.Combine(to, relativePath);

                if ((fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.CreateDirectory(destFileName);
                }
                else
                {
                    IFileTransformer transformer;
                    if (transformOptions.FileTransformers.TryGetValue(fileSystemInfo.Extension, out transformer))
                    {
                        var input = File.ReadAllText(fileSystemInfo.FullName);

                        var output = transformer.Transform(input);

                        File.WriteAllText(destFileName, output);

                        // File.Copy(fileSystemInfo.FullName, destFileName);
                    }
                }
            }
        }
        public static void CopyDirectory(string from, string to)
        {
            // var foldersToExclude = new[] { "\\docs", "\\bin", "\\obj", "\\packages", "\\.nuget", ".git", "\\.svn" };

            var directoryInfo = new DirectoryInfo(from);
            foreach (var fileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                var relativePath = GetRelativePath(directoryInfo.FullName, fileSystemInfo.FullName);
                /*if (foldersToExclude.Any(s => relativePath.Contains(s)))
                {
                    continue;
                }*/

                var destFileName = Path.Combine(to, relativePath);

                if ((fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.CreateDirectory(destFileName);
                }
                else
                {
                    File.Copy(fileSystemInfo.FullName, destFileName);
                }
            }
        }

        public static string GetRelativePath(string folderPath, string filePath)
        {
            var pathUri = new Uri(filePath);

            if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
            {
                folderPath += Path.DirectorySeparatorChar;
            }

            var folderUri = new Uri(folderPath);

            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
