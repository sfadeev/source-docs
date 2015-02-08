using System;
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
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRepositoryCatalog _repositoryCatalog;

        private Timer _timer;

        public RepositoryUpdater(IRepositoryCatalog repositoryCatalog)
        {
            _repositoryCatalog = repositoryCatalog;
        }

        public void Start()
        {
            _timer = new Timer(state => { UpdateRepositories(); }, null, 0, 30 * 1000);
        }

        public void UpdateRepositories()
        {
            if (Log.IsDebugEnabled) Log.Debug("Checking for updates in repositories.");

            var repositories = _repositoryCatalog.GetRepositories();

            foreach (var repository in repositories)
            {
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
