using System.Collections.Generic;
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
                return new
                {
                    RepoId = (string)x.repoId,
                    NodeName = (string)x.nodeName,
                    Children = GetTestRepoIndex((string)x.repoId, (string)x.nodeName)
                };

                // return Response.AsFile(".repos/index.json");
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
                new Repo { Id = "Twitter.Bootstrap.Less" }
            };
        }

        public static IList<IndexItem> GetTestRepoIndex(string repoId, string nodeName)
        {
            var result = new List<IndexItem>();

            for (var i = 0; i < 10; i++)
            {
                var item0 = new IndexItem
                {
                    Path = "item/" + i,
                    Name = repoId + "/" + nodeName + " Item " + i,
                    Children = new List<IndexItem>()
                };

                for (var j = 0; j < 5; j++)
                {
                    var item1 = new IndexItem
                    {
                        Path = item0.Path + "/" + j,
                        Name = item0.Name + "." + j,
                        Children = new List<IndexItem>()
                    };

                    for (var k = 0; k < 3; k++)
                    {
                        var item2 = new IndexItem
                        {
                            Path = item1.Path + "/" + k,
                            Name = item1.Name + "." + k
                        };

                        item1.Children.Add(item2);
                    }

                    item0.Children.Add(item1);
                }

                result.Add(item0);
            }

            return result;
        }
    }
}