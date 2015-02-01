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
                IRepositoryCatalog repositoryCatalog = 
                    new DefaultRepositoryCatalog(Path.Combine(Response.RootPath, ".repos/"));

                return repositoryCatalog.GetRepos();
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
                        + File.ReadAllText(Path.Combine(Response.RootPath, ".repos/README.1.md"))
                };
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