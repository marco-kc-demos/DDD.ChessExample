using DDD.Core.Application.EventStoreModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDD.Core.Application
{
    public abstract class EventStoreRepository<TId, TRoot> : IEventStoreRepository<TId, TRoot> 
        where TRoot : AggregateRoot<TId>
    {
        protected static readonly TypeCache _typeCache;
        private static readonly JsonSerializerSettings _serializerSettings;
        #region static constructor
        static EventStoreRepository()
        {
            _typeCache = new TypeCache();

            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new DefaultNamingStrategy()
            });
        }
        #endregion
        private readonly DbContextOptions _options;

        public EventStoreRepository(DbContextOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// In a derived class, this method should be implemented to provide an instance
        ///     of the derived EventStoreContext that matches the derived EventStoreRepository.
        /// (Factory Method Design Pattern)
        /// </summary>
        /// <param name="options">The Context Options used by the Entity Framework.</param>
        /// <returns>An EventStoreContext that matches this concrete EventStoreRepository.</returns>
        protected abstract EventStoreContext<TId, TRoot> CreateContext(DbContextOptions options);


        public async Task<TRoot> CreateAsync()
        {
            using (var context = CreateContext(_options))
            {
                await context.Database.EnsureCreatedAsync();

                var newRoot = new RootModel<TId>
                {
                    Version = 0,
                    Events = new List<EventModel>(),
                };
                context.AggregateRoots.Add(newRoot);

                await context.SaveChangesAsync();

                TRoot result = CreateAggregateRoot(newRoot.Id);
                return result;
            }
        }

        public virtual async Task SaveAsync(TRoot root)
        {
            var events = root.Events.Select(e => SerializeEvent(e, root.Version)).ToList();

            using (var context = CreateContext(_options))
            {
                await context.Database.EnsureCreatedAsync();

                var aggregateRoot = await context.AggregateRoots.SingleOrDefaultAsync(g => g.Id.Equals(root.Id));

                if (aggregateRoot == null)
                {
                    aggregateRoot = new RootModel<TId>
                    {
                        Id = root.Id,
                        Version = root.Version,
                        Events = events,
                    };
                    context.AggregateRoots.Add(aggregateRoot);
                }
                else
                {
                    context.Entry(aggregateRoot).OriginalValues["Version"] = root.OriginalVersion;
                    aggregateRoot.Version = root.Version;
                    aggregateRoot.Events = events;
                }
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task<TRoot> FindAsync(TId id)
        {
            using (var context = CreateContext(_options))
            {
                context.Database.EnsureCreated();

                var aggregateRoot = await context.AggregateRoots.SingleOrDefaultAsync(g => g.Id.Equals(id));

                if (aggregateRoot == null)
                {
                    return null;
                }

                await context.Entry(aggregateRoot).Collection(gr => gr.Events).LoadAsync();
                IEnumerable<DomainEvent> domainEvents = aggregateRoot.Events.Select(DeserializeEvent);

                TRoot result = CreateAggregateRoot(id, domainEvents);
                return result;
            }
        }


        /// <summary>
        /// Create an aggregate root object from an id.
        /// </summary>
        /// <param name="id">the id (from persistent store) of the aggregate root object</param>
        /// <returns>an new aggregate root object</returns>
        protected virtual TRoot CreateAggregateRoot(TId id)
        {
            TRoot root = (TRoot) Activator.CreateInstance(typeof(TRoot), id);
            return root;
        }

        /// <summary>
        /// Create an aggregate root object from an id and a list of events.
        /// </summary>
        /// <param name="id">the id (from persistent store) of the aggregate root object</param>
        /// <param name="domainEvents">the events (from persistent store) to replay against the aggregate root object</param>
        /// <returns>an up-to-date aggregate root object</returns>
        protected virtual TRoot CreateAggregateRoot(TId id, IEnumerable<DomainEvent> domainEvents)
        {
            TRoot root = (TRoot)
                Activator.CreateInstance(typeof(TRoot), id, domainEvents);
            return root;
        }

        /// <summary>
        /// Serializes a domain event into a EventModel (for storing in persistent store)
        /// </summary>
        /// <param name="domainEvent">the domain event that is to be serialized</param>
        /// <param name="version">The consistent version of the aggregate root to which the domain event attributes</param>
        /// <returns>an entity for in the persistent store</returns>
        protected virtual EventModel SerializeEvent(DomainEvent domainEvent, int version)
        {
            var evt = new EventModel
            {
                Version = version,
                EventType = domainEvent.GetType().FullName,
                EventData = JsonConvert.SerializeObject(domainEvent, _serializerSettings),
            };
            return evt;
        }

        /// <summary>
        /// Deserializes an eventmodel into a domain event.
        /// It uses a Tye Cache to resolve types from other assemblies.
        /// </summary>
        /// <param name="evt">an EventModel entity (from the persistent store)</param>
        /// <returns>a typed domain event</returns>
        protected virtual DomainEvent DeserializeEvent(EventModel evt)
        {
            Type eventType = _typeCache.FindType(evt.EventType);
            JObject obj = JsonConvert.DeserializeObject<JObject>(evt.EventData, _serializerSettings);
            DomainEvent domainEvent = obj.ToObject(eventType) as DomainEvent;
            return domainEvent;
        }
    }
}
