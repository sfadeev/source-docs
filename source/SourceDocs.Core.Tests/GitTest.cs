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
            var gitSettings = new GitRepositoryHandler.Settings
            {
                Url = repoUrl,
                ConfigFile = Path.Combine(FileHelper.GetWorkingDir("./repos/", repoUrl), "config.json"),
                WorkingDirectory = FileHelper.GetWorkingDir("./repos/", repoUrl, "repo")
            };

            using (var repo = new GitRepositoryHandler(gitSettings))
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
                        var options = new TransformOptions
                        {
                            TempDirectory = FileHelper.GetWorkingDir("./repos/", gitSettings.Url, "temp"),
                            WorkingDirectory = gitSettings.WorkingDirectory,
                            OutputDirectory = FileHelper.GetWorkingDir("./repos/", gitSettings.Url, "docs", node.Name),
                            ExcludeDirectories = new[] { "docs", "bin", "obj", "packages", ".nuget", ".git", ".svn" },
                            FileTransformers = new Dictionary<string, IFileTransformer>
                            {
                                { ".md", new MarkdownFileTransformer() },
                                { ".cs", new SourceFileTransformer() }
                            }
                        };

                        new RepositoryTransformer(new DefaultJavaScriptSerializer()).Transform(options);

                        // update repo config
                        node.Generated = node.Updated;
                        serialize(gitSettings.ConfigFile, config);
                    }

                    Console.Out.Flush();
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
