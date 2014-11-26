using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using SourceDocs.Core.Generation;

namespace SourceDocs.Core.Tests
{
    [TestFixture]
    public class GitTest
    {
        [TestCase("https://github.com/sfadeev/renocco.git")]
        [TestCase("https://github.com/sfadeev/source-docs.git")]
        [TestCase("c:\\data\\projects\\temp\\SomeRepo")]
        public void WorkflowTest(string repoUrl)
        {
            var repoDir = GetWorkingDir("./repos/", repoUrl, "repo");
            var configFile = Path.Combine(GetWorkingDir("./repos/", repoUrl), "config.json");

            var repo = new GitRepository(repoUrl, repoDir);

            var config = File.Exists(configFile)
                ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile))
                : new Config { Url = repoUrl };

            Action<string> update = branchName =>
            {
                var commit = repo.Update(branchName);

                var branch = config.Branches[branchName] = new Branch();
                if (commit != null)
                {
                    branch.Updated = commit.Committer.When;
                }

                File.WriteAllText(configFile,
                    JsonConvert.SerializeObject(config, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
            };

            foreach (var branchName in repo.GetBranches())
            {
                update(branchName);
            }

            while (true)
            {
                foreach (var branchName in repo.GetBranches(changedOnly: true))
                {
                    update(branchName);
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

    public class Config
    {
        public Config()
        {
            Branches = new Dictionary<string, Branch>();
        }

        public string Url { get; set; }

        public IDictionary<string, Branch> Branches { get; set; }
    }

    public class Branch
    {
        public DateTimeOffset Updated { get; set; }
    }
}
