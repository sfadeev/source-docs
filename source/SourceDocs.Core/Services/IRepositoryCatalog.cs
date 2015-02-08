using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Serialization;
using SourceDocs.Core.Helpers;
using SourceDocs.Core.Models;

namespace SourceDocs.Core.Services
{
    public interface IRepositoryCatalog
    {
        IRepository[] GetRepositories();

        Repos GetRepos();
    }

    public class DefaultRepositoryCatalog : IRepositoryCatalog
    {
        private readonly object _repositoriesLock = new object();

        private readonly IContextProvider _contextProvider;
        private readonly IJavaScriptSerializer _javaScriptSerializer;

        private IRepository[] _repositories;

        public DefaultRepositoryCatalog(IContextProvider contextProvider, IJavaScriptSerializer javaScriptSerializer)
        {
            _contextProvider = contextProvider;
            _javaScriptSerializer = javaScriptSerializer;
        }

        public IRepository[] GetRepositories()
        {
            if (_repositories == null)
            {
                lock (_repositoriesLock)
                {
                    if (_repositories == null)
                    {
                        _repositories = LoadRepositories();
                    }
                }
            }

            return _repositories;
        }

        private IRepository[] LoadRepositories()
        {
            var reposConfigPath = _contextProvider.MapPath("repos.json");
            var reposStream = File.ReadAllText(reposConfigPath);
            var repos = _javaScriptSerializer.Deserialize<Repos>(reposStream);

            var result = new List<IRepository>();

            foreach (var item in repos.Items)
            {
                var repoDir = FileHelper.GetWorkingDir(_contextProvider.MapPath("./repos/"), item.Url, "repo");

                var gitSettings = new GitRepository.Settings
                {
                    Url = item.Url,
                    WorkingDirectory = repoDir
                };

                result.Add(new GitRepository(gitSettings));
            }

            return result.ToArray();
        }

        public Repos GetRepos()
        {
            var reposConfigPath = _contextProvider.MapPath("repos.json");
            var reposStream = File.ReadAllText(reposConfigPath);
            var result = _javaScriptSerializer.Deserialize<Repos>(reposStream);

            foreach (var item in result.Items)
            {
                item.Nodes = new[]
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
                };
            }

            return result;
        }
    }
}
