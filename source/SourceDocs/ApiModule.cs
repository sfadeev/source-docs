using System.Collections.Generic;
using System.IO;
using Nancy;
using SourceDocs.Core.Models;
using SourceDocs.Core.Services;

namespace SourceDocs
{
    public class ApiModule : NancyModule
    {
        public ApiModule(IRepositoryCatalog repositoryCatalog) : base("/api")
        {
            Get["/repositories"] = parameters =>
            {
                return repositoryCatalog.GetRepos();
            };

            Get["/repositories/{repoId}/{nodeName*}/index"] = x =>
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

            Get["/repositories/{repoId}/{nodeName*}/document/{path*}"] = x =>
            {
                return new
                {
                    Content =
                        "<h1>" + x.repoId + "/" + x.nodeName + "/" + x.path + "</h1>"
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