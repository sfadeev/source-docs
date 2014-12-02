using System;
using System.Collections.Generic;
using System.Globalization;
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
        [TestCase("https://github.com/progit/progit2.git")]
        [TestCase("https://hg@bitbucket.org/tortoisehg/thg")]
        [TestCase("http://repo.or.cz/sqlgg.git")]
        [TestCase("c:\\data\\projects\\temp\\SomeRepo")]
        public void WorkflowTest(string repoUrl)
        {
            var repoDir = GetWorkingDir("./repos/", repoUrl, "repo");
            var configFile = Path.Combine(GetWorkingDir("./repos/", repoUrl), "config.json");

            var repo = new GitRepository(repoUrl, repoDir);

            var config = File.Exists(configFile)
                ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile))
                : new Config { Url = repoUrl };

            Action writeConfig = () =>
            {
                var serializeObject = JsonConvert.SerializeObject(config, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                File.WriteAllText(configFile, serializeObject);
            };

            Action<string> update = branchName =>
            {
                var commit = repo.Update(branchName);

                var branch = config.Branches[branchName] = new Branch();
                if (commit != null)
                {
                    branch.Updated = commit.Committer.When;
                }

                writeConfig();

                Thread.Sleep(1000);
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

                foreach (var branch in config.Branches)
                {
                    if (branch.Value.Generated == null || branch.Value.Generated < branch.Value.Updated)
                    {
                        repo.Checkout(branch.Key);

                        var tempDir = GetWorkingDir("./repos/", repoUrl, "temp");
                        Console.WriteLine("Generating docs for {0} in {1}", branch.Key, tempDir);
                        Empty(tempDir);
                        CopyDirs(repoDir, tempDir); // generate

                        var outDir = GetWorkingDir("./repos/", repoUrl, "docs", branch.Key);
                        Console.WriteLine("Copying docs for {0} to {1}", branch.Key, outDir);
                        Empty(outDir);
                        CopyDirs(tempDir, outDir); // ready docs

                        Empty(tempDir);

                        branch.Value.Generated = branch.Value.Updated;

                        writeConfig();
                    }
                }

                Thread.Sleep(1000);
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

        public static void Empty(string directoryPath)
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

        public static void CopyDirs(string from, string to)
        {
            var foldersToExclude = new[] { "\\docs", "\\bin", "\\obj", "\\packages", "\\.nuget", ".git", "\\.svn" };

            var directoryInfo = new DirectoryInfo(from);
            foreach (var fileSystemInfo in directoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                var relativePath = GetRelativePath(directoryInfo.FullName, fileSystemInfo.FullName);
                if (foldersToExclude.Any(s => relativePath.Contains(s)))
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

        public DateTimeOffset? Generated { get; set; }
    }
}
