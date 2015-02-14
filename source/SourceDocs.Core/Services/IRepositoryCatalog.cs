using System.Collections.Generic;
using System.IO;
using System.Linq;
using SourceDocs.Core.Helpers;
using SourceDocs.Core.Models;

namespace SourceDocs.Core.Services
{
    public interface IRepositoryCatalog
    {
        IRepository[] GetRepositories();

        Repos GetRepos();

        void UpdateNodes(string repositoryUrl, IList<Node> nodes);
    }

    public class RepositoryCatalog : IRepositoryCatalog
    {
        private readonly object _repositoriesLock = new object();
        private readonly object _reposLock = new object();

        private readonly IContextProvider _contextProvider;
        private readonly IJavaScriptSerializer _javaScriptSerializer;

        private IRepository[] _repositories;
        private Repos _repos;

        public RepositoryCatalog(IContextProvider contextProvider, IJavaScriptSerializer javaScriptSerializer)
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
            var repos = GetRepos();

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
            if (_repos == null)
            {
                lock (_reposLock)
                {
                    if (_repos == null)
                    {
                        _repos = _javaScriptSerializer.Deserialize<Repos>(
                            File.ReadAllText(_contextProvider.MapPath("repos.json")));
                    }
                }
            }

            return _repos;
        }

        public void UpdateNodes(string repositoryUrl, IList<Node> nodes)
        {
            lock (_reposLock)
            {
                var repo = GetRepos().Items.FirstOrDefault(x => x.Url == repositoryUrl);
                if (repo != null)
                {
                    repo.Nodes = nodes;
                }
            }
        }
    }
}
