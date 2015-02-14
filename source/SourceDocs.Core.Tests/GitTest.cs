using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SourceDocs.Core.Helpers;
using SourceDocs.Core.Models;
using SourceDocs.Core.Services;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class GitTest
    {
        [TestCase("https://github.com/libgit2/libgit2sharp.git")]
        [TestCase("https://github.com/sfadeev/renocco.git")]
        [TestCase("git://github.com/sfadeev/source-docs.git")]
        [TestCase("c:\\data\\projects\\temp\\SomeRepo")]
        public void WorkflowTest(string repoUrl)
        {
            var repoDir = FileHelper.GetWorkingDir("./repos/", repoUrl, "repo");
            var configFile = Path.Combine(FileHelper.GetWorkingDir("./repos/", repoUrl), "config.json");

            var gitSettings = new GitRepository.Settings
            {
                Url = repoUrl,
                WorkingDirectory = repoDir
            };

            using (var repo = new GitRepository(gitSettings))
            {
                var config = File.Exists(configFile)
                    ? JsonConvert.DeserializeObject<Repo>(File.ReadAllText(configFile))
                    : new Repo();

                Action<string, object> serialize = (path, value) =>
                {
                    var serializeObject = JsonConvert.SerializeObject(value, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });

                    File.WriteAllText(path, serializeObject);
                };

                while (true)
                {
                    config.Nodes = repo.UpdateNodes(config.Nodes);

                    // todo: write config if something changed
                    serialize(configFile, config);

                    Console.Write("."); // for pretty test ;)

                    Node node;
                    while ((node = config.Nodes.FirstOrDefault(x => x.Generated == null || x.Updated > x.Generated)) != null)
                    {
                        repo.UpdateNode(node);

                        // generate docs
                        var tempDir = FileHelper.GetWorkingDir("./repos/", gitSettings.Url, "temp");
                        Console.WriteLine("Generating docs for {0} in {1}", node.Name, tempDir);
                        EmptyDirectory(tempDir);

                        var index = Transform(gitSettings.WorkingDirectory, tempDir, new TransformOptions
                        {
                            ExcludeDirectories = new[] { "docs", "bin", "obj", "packages", ".nuget", ".git", ".svn" },
                            FileTransformers = new Dictionary<string, IFileTransformer>
                            {
                                { ".md", new MarkdownFileTransformer() },
                                { ".cs", new SourceFileTransformer() }
                            }
                        });

                        serialize(Path.Combine(tempDir, "index.json"), index);

                        var outDir = FileHelper.GetWorkingDir("./repos/", gitSettings.Url, "docs", node.Name);
                        Console.WriteLine("Copying docs for {0} to {1}", node.Name, outDir);
                        EmptyDirectory(outDir);
                        CopyDirectory(tempDir, outDir); // ready docs

                        EmptyDirectory(tempDir);

                        node.Generated = node.Updated;

                        serialize(configFile, config);
                    }

                    Console.Out.Flush();
                    Thread.Sleep(5000);
                }
            }
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

        public static IList<IndexItem> Transform(string from, string to, TransformOptions transformOptions)
        {
            if (transformOptions == null) throw new ArgumentNullException("transformOptions");

            var index = new List<IndexItem>();

            var directoryInfo = new DirectoryInfo(from);

            foreach (var fileInfo in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = GetRelativePath(directoryInfo.FullName, fileInfo.FullName);

                // todo: use more complex match with * instead of StartsWith
                if (transformOptions.ExcludeDirectories.Any(relativePath.StartsWith)) continue;
                
                IFileTransformer transformer;
                if (transformOptions.FileTransformers.TryGetValue(fileInfo.Extension, out transformer))
                {
                    index.Add(new IndexItem { Name = Path.GetFileName(relativePath), Path = relativePath.Replace('\\', '/') });

                    var destFileName = Path.Combine(to, relativePath);
                    var destDirectoryName = Path.GetDirectoryName(destFileName);

                    if (Directory.Exists(destDirectoryName) == false)
                        Directory.CreateDirectory(destDirectoryName);

                    var input = File.ReadAllText(fileInfo.FullName);
                    var output = transformer.Transform(input);

                    File.WriteAllText(destFileName, output);
                }
            }

            return index;
        }

        public static void CopyDirectory(string from, string to)
        {
            var directoryInfo = new DirectoryInfo(from);

            // todo: enumerate only files or merge with Transform method
            foreach (var fileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                var relativePath = GetRelativePath(directoryInfo.FullName, fileSystemInfo.FullName);

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
