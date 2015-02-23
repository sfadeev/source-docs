using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SourceDocs.Core.Helpers;
using SourceDocs.Core.Models;

namespace SourceDocs.Core.Services
{
    public interface IStartable
    {
        void Start();
    }

    public class RepositoryUpdater : IStartable, IDisposable
    {
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRepositoryCatalog _repositoryCatalog;
        private readonly IRepositoryTransformer _repositoryTransformer;
        private readonly IContextProvider _contextProvider;
        private readonly INotificationService _notificationService;

        private readonly object _repositoryLock = new object();
        private volatile bool _repositoryLocked;

        private Timer _timer;

        public RepositoryUpdater(IContextProvider contextProvider, INotificationService notificationService,
            IRepositoryCatalog repositoryCatalog, IRepositoryTransformer repositoryTransformer)
        {
            _contextProvider = contextProvider;
            _notificationService = notificationService;
            _repositoryCatalog = repositoryCatalog;
            _repositoryTransformer = repositoryTransformer;
        }

        public void Start()
        {
            _timer = new Timer(state =>
            {
                if (_repositoryLocked == false)
                {
                    lock (_repositoryLock)
                    {
                        if (_repositoryLocked == false)
                        {
                            try
                            {
                                _repositoryLocked = true;

                                try
                                {
                                    UpdateRepositories();
                                }
                                catch (Exception ex)
                                {
                                    if (Log.IsErrorEnabled)
                                    {
                                        Log.Error("Failed to update repositories.", ex);
                                    }
                                }
                            }
                            finally
                            {
                                _repositoryLocked = false;
                            }
                        }
                    }
                }
            }, null, 0, 60 * 1000);
        }

        public void UpdateRepositories()
        {
            _notificationService.Notify("Checking for updates in repositories ...");

            var repositories = _repositoryCatalog.GetRepositories();

            foreach (var repository in repositories)
            {
                try
                {
                    _notificationService.Notify("Updating " + repository.Url);

                    var settings = _repositoryCatalog.GetRepositoryConfig(repository.Url);
                    if (settings == null) continue;

                    var repo = _repositoryCatalog.GetRepos().Single(x => x.Url == settings.Url);

                    var nodes = repository.UpdateNodes(repo.Nodes);

                    _repositoryCatalog.UpdateRepositoryConfig(repository.Url, config => config.Nodes = nodes);

                    _notificationService.Notify("Updated " + repository.Url + " nodes: " + string.Join(", ", nodes.Select(x => x.Name)));

                    Node node;
                    while ((node = nodes.FirstOrDefault(x => x.Generated == null || x.Updated > x.Generated)) != null)
                    {
                        repository.UpdateNode(node);

                        _notificationService.Notify("Generating documentation for " + repository.Url + " node: " + node.Name, NotificationType.Success);

                        // generate docs
                        var options = new TransformOptions
                        {
                            WorkingDirectory = settings.WorkingDirectory,
                            TempDirectory = FileHelper.GetWorkingDir(_contextProvider.MapPath("."), settings.Url, "temp"),
                            OutputDirectory = FileHelper.GetWorkingDir(_contextProvider.MapPath("."), settings.Url, "docs", node.Name),
                            ExcludeDirectories = new[] { "docs", "bin", "obj", "packages", ".nuget", ".git", ".svn" },
                            FileTransformers = new Dictionary<string, IFileTransformer>
                            {
                                { ".md", new MarkdownFileTransformer() },
                                { ".cs", new SourceFileTransformer() },
                                { ".js", new SourceFileTransformer() }
                            }
                        };

                        _repositoryTransformer.Transform(options);

                        node.Generated = node.Updated;
                        _repositoryCatalog.UpdateRepositoryConfig(repository.Url, null);
                    }
                }
                catch (Exception ex)
                {
                    if (Log.IsErrorEnabled)
                    {
                        Log.Error("Failed to update repository " + repository.Url, ex);
                    }

                    _notificationService.Notify("Failed to update repository " + repository.Url + " : " + ex.Message, NotificationType.Error);
                }

                Thread.Sleep(5 * 1000);
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
