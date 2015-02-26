using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SourceDocs.Core.Helpers;
using SourceDocs.Core.Models;

namespace SourceDocs.Core.Services
{
    public interface IRepositoryCatalog
    {
        IRepositoryHandler[] GetRepositories();

        Repo[] GetRepos();

        GitRepositoryHandler.Settings GetRepositoryConfig(string repositoryUrl);

        void UpdateRepositoryConfig(string repositoryUrl, Action<Repo> updateProperties);
    }

    public class RepositoryCatalog : IRepositoryCatalog
    {
        private readonly object _reposLock = new object();

        private readonly IContextProvider _contextProvider;
        private readonly IJavaScriptSerializer _javaScriptSerializer;

        private IDictionary<Repo, GitRepositoryHandler.Settings> _repoMap;

        public RepositoryCatalog(IContextProvider contextProvider, IJavaScriptSerializer javaScriptSerializer)
        {
            _contextProvider = contextProvider;
            _javaScriptSerializer = javaScriptSerializer;
        }

        public IRepositoryHandler[] GetRepositories()
        {
            return LoadRepoMap().Where(x => x.Value != null).Select(x => (IRepositoryHandler)new GitRepositoryHandler(x.Value)).ToArray();
        }

        public Repo[] GetRepos()
        {
            return LoadRepoMap().Keys.ToArray();
        }

        private IDictionary<Repo, GitRepositoryHandler.Settings> LoadRepoMap()
        {
            if (_repoMap == null)
            {
                lock (_reposLock)
                {
                    if (_repoMap == null)
                    {
                        var repos = _javaScriptSerializer.Deserialize<RepoConfig>(
                            File.ReadAllText(_contextProvider.MapPath("repositories.json"))).Repositories;

                        _repoMap = new Dictionary<Repo, GitRepositoryHandler.Settings>();

                        foreach (var repo in repos)
                        {
                            GitRepositoryHandler.Settings settings = null;

                            if (repo.Id != null && repo.Url != null)
                            {
                                settings = new GitRepositoryHandler.Settings
                                {
                                    Url = repo.Url,
                                    BaseDirectory = FileHelper.GetWorkingDir(_contextProvider.MapPath("."), repo.Url),
                                    ConfigFile = Path.Combine(FileHelper.GetWorkingDir(_contextProvider.MapPath("."), repo.Url), "config.json"),
                                    WorkingDirectory = FileHelper.GetWorkingDir(_contextProvider.MapPath("."), repo.Url, "repo")
                                };
                            }

                            var key = settings != null && File.Exists(settings.ConfigFile)
                                ? _javaScriptSerializer.Deserialize<Repo>(File.ReadAllText(settings.ConfigFile))
                                : repo;

                            _repoMap[key] = settings;
                        }
                    }
                }
            }

            return _repoMap;
        }

        public GitRepositoryHandler.Settings GetRepositoryConfig(string repositoryUrl)
        {
            lock (_reposLock)
            {
                var repoMap = LoadRepoMap();

                var repo = repoMap.Keys.FirstOrDefault(x => x.Url == repositoryUrl);

                return repo != null ? repoMap[repo] : null;
            }
        }

        public void UpdateRepositoryConfig(string repositoryUrl, Action<Repo> updateProperties)
        {
            lock (_reposLock)
            {
                var repoMap = LoadRepoMap();

                var repo = repoMap.Keys.FirstOrDefault(x => x.Url == repositoryUrl);
                if (repo != null)
                {
                    if (updateProperties != null) updateProperties(repo);

                    var settings = repoMap[repo];
                    File.WriteAllText(settings.ConfigFile, _javaScriptSerializer.Serialize(repo));
                }
            }
        }
    }
}
