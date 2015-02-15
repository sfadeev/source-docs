using System;
using System.Collections.Generic;
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
            var gitSettings = new GitRepository.Settings
            {
                Url = repoUrl,
                ConfigFile = Path.Combine(FileHelper.GetWorkingDir("./repos/", repoUrl), "config.json"),
                WorkingDirectory = FileHelper.GetWorkingDir("./repos/", repoUrl, "repo")
            };

            using (var repo = new GitRepository(gitSettings))
            {
                var config = File.Exists(gitSettings.ConfigFile)
                    ? JsonConvert.DeserializeObject<Repo>(File.ReadAllText(gitSettings.ConfigFile))
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
                    serialize(gitSettings.ConfigFile, config);

                    Console.Write("."); // for pretty test ;)

                    Node node;
                    while ((node = config.Nodes.FirstOrDefault(x => x.Generated == null || x.Updated > x.Generated)) != null)
                    {
                        repo.UpdateNode(node);

                        // generate docs
                        var tempDir = FileHelper.GetWorkingDir("./repos/", gitSettings.Url, "temp");
                        Console.WriteLine("Generating docs for {0} in {1}", node.Name, tempDir);
                        FileHelper.EmptyDirectory(tempDir);

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
                        FileHelper.EmptyDirectory(outDir);

                        FileHelper.CopyDirectory(tempDir, outDir); // ready docs

                        FileHelper.EmptyDirectory(tempDir);

                        // update repo config
                        node.Generated = node.Updated;
                        serialize(gitSettings.ConfigFile, config);
                    }

                    Console.Out.Flush();
                    Thread.Sleep(5000);
                }
            }
        }

        public static IList<IndexItem> Transform(string from, string to, TransformOptions transformOptions)
        {
            if (transformOptions == null) throw new ArgumentNullException("transformOptions");

            var index = new List<IndexItem>();

            var directoryInfo = new DirectoryInfo(from);

            foreach (var fileInfo in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var relativePath = FileHelper.GetRelativePath(directoryInfo.FullName, fileInfo.FullName);

                // todo: use more complex match with * instead of StartsWith
                if (transformOptions.ExcludeDirectories.Any(relativePath.StartsWith)) continue;
                
                IFileTransformer transformer;
                if (transformOptions.FileTransformers.TryGetValue(fileInfo.Extension, out transformer))
                {
                    index.Add(new IndexItem { Name = Path.GetFileName(relativePath), Path = relativePath.Replace('\\', '/') });

                    var destFileName = Path.Combine(to, relativePath);
                    var destDirectoryName = Path.GetDirectoryName(destFileName);

                    FileHelper.EnsureDirectoryExists(destDirectoryName);

                    var input = File.ReadAllText(fileInfo.FullName);
                    var output = transformer.Transform(input);

                    File.WriteAllText(destFileName, output);
                }
            }

            return index;
        }
    }
}
