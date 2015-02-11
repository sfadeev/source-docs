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

        private Timer _timer;

        public RepositoryUpdater(IRepositoryCatalog repositoryCatalog, INotificationService notificationService)
        {
            _repositoryCatalog = repositoryCatalog;
            _notificationService = notificationService;
        }

        public void Start()
        {
            _timer = new Timer(state => { UpdateRepositories(); }, null, 0, 60 * 1000);
        }

        public void UpdateRepositories()
        {
            _notificationService.Notify("Checking for updates in repositories.");

            var repositories = _repositoryCatalog.GetRepositories();

            foreach (var repository in repositories)
            {
                _notificationService.Notify("Updating repository " + repository.Url);

                repository.UpdateNodes();
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
