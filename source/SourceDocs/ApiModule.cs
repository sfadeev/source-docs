using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.Helpers;
using SourceDocs.Core.Models;
using SourceDocs.Core.Services;

namespace SourceDocs
{
    public class ApiModule : NancyModule
    {
        public ApiModule(IRepositoryCatalog repositoryCatalog, IJavaScriptSerializer javaScriptSerializer) : base("/api")
        {
            Get["/repositories"] = parameters =>
            {
                return repositoryCatalog.GetRepos();
            };

            Get["/repositories/{repoId}/{nodeName*}/index"] = x =>
            {
                var repo = repositoryCatalog.GetRepos().Single(r => r.Id == x.repoId);
                var config = repositoryCatalog.GetRepositoryConfig(repo.Url);
                var path = Path.Combine(config.BaseDirectory, "docs", x.nodeName, "index.json");

                var result = new IndexItem
                {
                    Path = "#",
                    Name = x.repoId + "/" + x.nodeName,
                    Children = File.Exists(path)
                        ? javaScriptSerializer.Deserialize<List<IndexItem>>(File.ReadAllText(path))
                        : new List<IndexItem>()
                };

                // BuildRepoIndex(result, 5);

                return result;
            };

            Get["/repositories/{repoId}/{nodeName*}/document/{path*}"] = x =>
            {
                var repo = repositoryCatalog.GetRepos().Single(r => r.Id == x.repoId);
                var config = repositoryCatalog.GetRepositoryConfig(repo.Url);
                var path = Path.Combine(config.BaseDirectory, "docs", x.nodeName, HttpUtility.UrlDecode(x.path));

                return new
                {
                    Content = File.Exists(path) ? File.ReadAllText(path) : string.Empty
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