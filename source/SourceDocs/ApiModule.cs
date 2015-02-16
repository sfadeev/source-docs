﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
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
                var indexPath = Path.Combine(config.BaseDirectory, "docs", x.nodeName, "index.json");

                var result = new IndexItem
                {
                    Path = "item",
                    Name = x.repoId + "/" + x.nodeName,
                    Children = javaScriptSerializer.Deserialize<List<IndexItem>>(
                        File.ReadAllText(indexPath))
                };

                // BuildRepoIndex(result, 5);

                return result;

                // return Response.AsFile(".repos/index.json");
            };

            Get["/repositories/{repoId}/{nodeName*}/document/{path*}"] = x =>
            {
                var repo = repositoryCatalog.GetRepos().Single(r => r.Id == x.repoId);
                var config = repositoryCatalog.GetRepositoryConfig(repo.Url);
                var path = Path.Combine(config.BaseDirectory, "docs", x.nodeName, x.path);


                return new
                {
                    Content = File.ReadAllText(path)
                        /*"<h1>" + x.repoId + "/" + x.nodeName + "/" + x.path + "</h1>"
                        + File.ReadAllText(Path.Combine(Response.RootPath, ".repos/README.1.md"))*/
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