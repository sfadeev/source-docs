using System.Collections.Concurrent;
using SourceDocs.Core.Events;

namespace SourceDocs.Core.Services
{
    public interface ITaskManager
    {
        void AddTask(IRepositoryTask task);
    }

    public class DefaultTaskManager : ITaskManager
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly ConcurrentQueue<IRepositoryTask> _queue = new ConcurrentQueue<IRepositoryTask>();

        public DefaultTaskManager(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void AddTask(IRepositoryTask task)
        {
            _queue.Enqueue(task);

            _eventAggregator.Publish(new TaskAdded());
        }

        public void GetTask()
        {
            IRepositoryTask task;
            if (_queue.TryDequeue(out task))
            {
            }
        }
    }
}
