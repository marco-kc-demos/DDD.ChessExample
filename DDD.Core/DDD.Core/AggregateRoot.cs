using System;
using System.Collections.Generic;

namespace DDD.Core
{
    /// <summary>
    /// Represents an aggregate-root of a domain aggregate (DDD). An aggregate-root is always an entity.
    /// </summary>
    /// <typeparam name="TId">The type of the Id of the entity.</typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        /// <summary>
        /// The list of events that occurred while handling commands.
        /// </summary>
        private readonly List<DomainEvent> _events;

        /// <summary>
        /// The list of events that occurred while handling commands.
        /// </summary>
        public IEnumerable<DomainEvent> Events => _events;

        /// <summary>
        /// Indication whether the aggregate is replaying events (true) or not (false).
        /// </summary>
        protected bool IsReplaying { get; private set; } = false;

        /// <summary>
        /// The current version after handling any commands.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// The original version of the aggregate after replaying all events in the event-store.
        /// </summary>
        public int OriginalVersion { get; private set; }

        /// <summary>
        /// Constructor for creating an empty aggregate.
        /// </summary>
        /// <param name="id">The unique id of the aggregate-root.</param>
        public AggregateRoot(TId id) : base(id)
        {
            OriginalVersion = 0;
            Version = 0;
            _events = new List<DomainEvent>();
        }

        /// <summary>
        /// Constructor for creating an aggregate of which the state is intialized by 
        /// replaying the list of events specified.
        /// </summary>
        /// <param name="id">The unique Id of the aggregate.</param>
        /// <param name="events">The events to replay.</param>
        public AggregateRoot(TId id, IEnumerable<DomainEvent> events) : this(id)
        {
            IsReplaying = true;
            foreach (DomainEvent evt in events)
            {
                When(evt);
                OriginalVersion++;
                Version++;
            }
            IsReplaying = false;
        }

        /// <summary>
        /// Let the aggregate handle an event and save it in the list of events 
        /// so it can be used outside the aggregate (persisted, published on a bus, ...).
        /// </summary>
        /// <param name="domainEvent">The event to handle.</param>
        /// <remarks>Use GetEvents to retrieve the list of events.</remarks>
        protected void RaiseEvent(DomainEvent domainEvent)
        {
            // let the derived aggregate handle the event
            When(domainEvent);

            // save the event so it can be published outside the aggregate
            _events.Add(domainEvent);
            Version += 1;
        }

        /// <summary>
        /// Clear the list of events that occurred while handling a command.
        /// </summary>
        public void ClearEvents()
        {
            _events.Clear();
        }

        /// <summary>
        /// Handle a specific event. Derived classes should  implement this method 
        ///for every event type.
        /// </summary>
        /// <param name="domainEevent">The event to handle.</param>
        /// <remarks>Because the parameter type of the specified event is dynamic, 
        /// the appropriate overload of the When method is called.</remarks>
        protected abstract void When(DomainEvent domainEvent);
    }
}