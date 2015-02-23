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

                IList<IndexItem> index = null;

                if (File.Exists(path))
                {
                    index = javaScriptSerializer.Deserialize<List<IndexItem>>(File.ReadAllText(path));
                }

                return new IndexItem
                {
                    Path = "#",
                    Name = x.repoId + " / " + x.nodeName,
                    Children = index
                };
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
    }
}