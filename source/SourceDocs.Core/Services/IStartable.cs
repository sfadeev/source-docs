using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;

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
        private readonly INotificationService _notificationService;

        private readonly object _updateLock = new object();
        private volatile bool _updateWorking;

        private Timer _timer;

        public RepositoryUpdater(IRepositoryCatalog repositoryCatalog, INotificationService notificationService)
        {
            _repositoryCatalog = repositoryCatalog;
            _notificationService = notificationService;
        }

        public void Start()
        {
            _timer = new Timer(state =>
            {
                if (_updateWorking == false)
                {
                    lock (_updateLock)
                    {
                        if (_updateWorking == false)
                        {
                            try
                            {
                                _updateWorking = true;

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
                                _updateWorking = false;
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

                    var nodes = repository.UpdateNodes();

                    _repositoryCatalog.UpdateNodes(repository.Url, nodes);

                    _notificationService.Notify("Updated " + repository.Url + " nodes: " + string.Join(", ", nodes.Select(x => x.Name)));
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
