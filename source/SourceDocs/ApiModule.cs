using Nancy;
using SourceDocs.Core;

namespace SourceDocs
{
    public class ApiModule : NancyModule
    {
        public ApiModule() : base("/api")
        {
            Get["/repos"] = parameters => new
            {
                Items = new[]
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
                }
            };
        }
    }
}