using System;
using System.Threading;

namespace SourceDocs.Core.Services
{
    public interface IStartable
    {
        void Start();
    }

    public class RepositoryUpdater : IStartable, IDisposable
    {
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

                                UpdateRepositories();
                            }
                            finally
                            {
                                _updateWorking = false;
                            }
                        }
                    }
                }
            }, null, 0, 30 * 1000);
        }

        public void UpdateRepositories()
        {
            _notificationService.Notify("Checking for updates in repositories ...");

            var repositories = _repositoryCatalog.GetRepositories();

            foreach (var repository in repositories)
            {
                _notificationService.Notify("Updating repository " + repository.Url);

                repository.UpdateNodes();

                Thread.Sleep(3 * 1000);
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
