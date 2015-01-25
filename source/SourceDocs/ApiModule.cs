using System.Collections.Generic;
using System.IO;
using Nancy;
using SourceDocs.Core;

namespace SourceDocs
{
    public class ApiModule : NancyModule
    {
        public ApiModule() : base("/api")
        {
            Get["/repos"] = parameters =>
            {
                return new
                {
                    Items = GetTestRepos()
                };
            };

            Get["/repos/{repoId}/{nodeName}/index"] = x =>
            {
                var result = new IndexItem
                {
                    Path = "item",
                    Name = x.repoId + "/" + x.nodeName,
                    Children = new List<IndexItem>()
                };

                BuildRepoIndex(result, 5);

                return result;

                // return Response.AsFile(".repos/index.json");
            };
            Get["/repos/{repoId}/{nodeName}/doc/{path*}"] = x =>
            {
                return new
                {
                    Content =
                        "<h1>Documentation for " + x.repoId + " v. " + x.nodeName + " - " + x.path + "</h1>"
                        + File.ReadAllText(Path.Combine(Response.RootPath, ".repos/README.3.md"))
                };
            };
        }

        public static Repo[] GetTestRepos()
        {
            return new[]
            {
                new Repo
                {
                    Id = "jQuery",
                    Nodes = new[]
                    {
                        new Node { Name = "2.1.3" },
                        new Node { Name = "2.1.1" },
                        new Node { Name = "2.1.0" },
                        new Node { Name = "2.0.3" },
                        new Node { Name = "2.0.2" },
                        new Node { Name = "2.0.1" },
                        new Node { Name = "2.0.0" },
                        new Node { Name = "1.11.2" },
                        new Node { Name = "1.11.1" },
                        new Node { Name = "1.11.0" }
                    }
                },
                new Repo { Id = "LibGit2Sharp" },
                new Repo
                {
                    Id = "log4net",
                    Nodes = new[]
                    {
                        new Node { Name = "1.2.13" },
                        new Node { Name = "1.2.12" },
                        new Node { Name = "1.2.11" },
                        new Node { Name = "1.2.10" }
                    }
                },
                new Repo { Id = "Nancy" },
                new Repo { Id = "Nancy.Hosting.Aspnet" },
                new Repo { Id = "Nancy.Viewengines.Razor" },
                new Repo
                {
                    Id = "Twitter.Bootstrap.Less",
                    Nodes = new[]
                    {
                        new Node { Name = "v1.0" }
                    }
                }
            };
        }

        public static void BuildRepoIndex(IndexItem parent, int level)
        {
            for (var i = 1; i < level; i++)
            {
                var item = new IndexItem
                {
                    Path = parent.Path + "/" + i,
                    Name = parent.Name + " Item " + i
                };

                parent.Children.Add(item);

                if (level > 0)
                {
                    item.Children = new List<IndexItem>();

                    BuildRepoIndex(item, level - 1);
                }
            }
        }
    }
}