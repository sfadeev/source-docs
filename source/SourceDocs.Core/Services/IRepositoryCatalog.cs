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

        Repo[] GetRepos();

        void UpdateNodes(string repositoryUrl, IList<Node> nodes);
    }

    public class RepositoryCatalog : IRepositoryCatalog
    {
        private readonly object _reposLock = new object();

        private readonly IContextProvider _contextProvider;
        private readonly IJavaScriptSerializer _javaScriptSerializer;

        private IDictionary<Repo, GitRepository.Settings> _repoMap;

        public RepositoryCatalog(IContextProvider contextProvider, IJavaScriptSerializer javaScriptSerializer)
        {
            _contextProvider = contextProvider;
            _javaScriptSerializer = javaScriptSerializer;
        }

        public IRepository[] GetRepositories()
        {
            return LoadRepoMap().Select(x => (IRepository)new GitRepository(x.Value)).ToArray();
        }

        public Repo[] GetRepos()
        {
            return LoadRepoMap().Keys.ToArray();
        }

        private IDictionary<Repo, GitRepository.Settings> LoadRepoMap()
        {
            if (_repoMap == null)
            {
                lock (_reposLock)
                {
                    if (_repoMap == null)
                    {
                        var repos = _javaScriptSerializer.Deserialize<Repos>(
                            File.ReadAllText(_contextProvider.MapPath("repositories.json"))).Items;

                        _repoMap = new Dictionary<Repo, GitRepository.Settings>();

                        foreach (var repo in repos)
                        {
                            var settings = new GitRepository.Settings
                            {
                                Url = repo.Url,
                                ConfigFile = Path.Combine(
                                    FileHelper.GetWorkingDir(_contextProvider.MapPath("."), repo.Url), "config.json"),
                                WorkingDirectory =
                                    FileHelper.GetWorkingDir(_contextProvider.MapPath("."), repo.Url, "repo")
                            };

                            var key = File.Exists(settings.ConfigFile)
                                ? _javaScriptSerializer.Deserialize<Repo>(File.ReadAllText(settings.ConfigFile))
                                : repo;

                            _repoMap[key] = settings;
                        }
                    }
                }
            }

            return _repoMap;
        }

        public void UpdateNodes(string repositoryUrl, IList<Node> nodes)
        {
            lock (_reposLock)
            {
                var repoMap = LoadRepoMap();

                var repo = repoMap.Keys.FirstOrDefault(x => x.Url == repositoryUrl);
                if (repo != null)
                {
                    repo.Nodes = nodes;

                    var settings = repoMap[repo];
                    File.WriteAllText(settings.ConfigFile, _javaScriptSerializer.Serialize(repo));
                }
            }
        }
    }
}
