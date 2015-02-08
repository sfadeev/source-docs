using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceDocs.Core.Services
{
    public interface IEventAggregator
    {
        ISubscriptionToken Subscribe<TEvent>(Action<TEvent> action, SubscriptionOptions options = null) where TEvent : IEvent;

		ISubscriptionToken Subscribe(Type eventType, Action<IEvent> action, SubscriptionOptions options = null);

		void Unsubscribe(ISubscriptionToken token);

        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;

		void Publish(Type eventType, IEvent @event);
	}

    public interface IEvent
    {
    }

	public class SubscriptionOptions
	{
		public bool WithInheritance { get; set; }
	}

    public interface ISubscriptionToken : IDisposable
    {
    }

    public class SubscriptionToken : IEquatable<SubscriptionToken>, ISubscriptionToken
    {
        private readonly Guid _token = Guid.NewGuid();

        private Action<SubscriptionToken> _unsubscribeAction;

        public SubscriptionToken(Action<ISubscriptionToken> unsubscribeAction)
        {
            _unsubscribeAction = unsubscribeAction;
        }

        public bool Equals(SubscriptionToken other)
        {
            return other != null && Equals(_token, other._token);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || Equals(obj as SubscriptionToken);
        }

        public override int GetHashCode()
        {
            return _token.GetHashCode();
        }

        public void Dispose()
        {
            if (_unsubscribeAction != null)
            {
                _unsubscribeAction(this);
                _unsubscribeAction = null;
            }
        }
    }

    public class EventAggregator : IEventAggregator
    {
        private readonly List<Subscription> _subscriptions = new List<Subscription>();

        public ISubscriptionToken Subscribe<TEvent>(Action<TEvent> action, SubscriptionOptions options = null) where TEvent : IEvent
        {
            return Subscribe(typeof(TEvent), o => action((TEvent)o), options);
        }

        public virtual ISubscriptionToken Subscribe(Type eventType, Action<IEvent> action, SubscriptionOptions options = null)
        {
            if (eventType == null) throw new ArgumentNullException("eventType");
            if (action == null) throw new ArgumentNullException("action");

            if (options == null) options = new SubscriptionOptions();

            var subscription = new Subscription(eventType, action, options, new SubscriptionToken(Unsubscribe));

            lock (_subscriptions)
            {
                _subscriptions.Add(subscription);
            }

            return subscription.Token;
        }

        public virtual void Unsubscribe(ISubscriptionToken token)
        {
            lock (_subscriptions)
            {
                var subscription = _subscriptions.FirstOrDefault(x => x.Token.Equals(token));
                if (subscription != null)
                {
                    _subscriptions.Remove(subscription);
                }
            }
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            Publish(typeof(TEvent), @event);
        }

        public void Publish(Type eventType, IEvent @event)
        {
            if (eventType == null) throw new ArgumentNullException("eventType");

            List<Subscription> subscriptions;

            lock (_subscriptions)
            {
                subscriptions = _subscriptions.Where(x => x.ShouldDeliver(eventType)).ToList();
            }

            foreach (var subscription in subscriptions)
            {
                subscription.Deliver(@event);
            }
        }

        public bool Contains(ISubscriptionToken token)
        {
            lock (_subscriptions)
            {
                return _subscriptions.Any(x => x.Token.Equals(token));
            }
        }

        private class Subscription
        {
            private readonly Type _eventType;
            private readonly Action<IEvent> _action;
            private readonly SubscriptionOptions _options;
            private readonly ISubscriptionToken _token;

            public Subscription(Type eventType, Action<IEvent> action, SubscriptionOptions options, ISubscriptionToken token)
            {
                _eventType = eventType;
                _action = action;
                _options = options;
                _token = token;
            }
            
            public ISubscriptionToken Token
            {
                get {  return _token; }
            }

            public bool ShouldDeliver(Type eventType)
            {
                return _eventType == eventType || (_options.WithInheritance && _eventType.IsAssignableFrom(eventType));
            }

            public void Deliver(IEvent @event)
            {
                _action(@event);
            }
        }
    }
}