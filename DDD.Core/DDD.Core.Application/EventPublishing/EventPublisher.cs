using Minor.Miffy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Core.Application
{
    public class EventPublisher<TConnection> : IEventPublisher
    {
        private readonly IBusContext<TConnection> _busContext;
        private static readonly JsonSerializerSettings _serializerSettings;
        #region static constructor
        static EventPublisher()
        {
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Formatting = Formatting.Indented;
            _serializerSettings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new DefaultNamingStrategy()
            });
        }
        #endregion

        public EventPublisher(IBusContext<TConnection> busContext)
        {
            _busContext = busContext;
        }

        public async Task PublishEventAsync(DomainEvent domainEvent)
        {
            // Do not publish internal events
            if (!(domainEvent is InternalDomainEvent))
            {
                using (IMessageSender sender = _busContext.CreateMessageSender())
                {
                    var message = ToEventMessage(domainEvent);
                    await sender.SendMessageAsync(message);
                }
            }       
        }

        public async Task PublishEventsAsync(IEnumerable<DomainEvent> domainEvents)
        {
            // Do not publish internal events
            var publicEvents = domainEvents.Where(evt => !(evt is InternalDomainEvent));
            if (publicEvents.Any())
            {
                using (IMessageSender sender = _busContext.CreateMessageSender())
                {
                    // The order of publihing the event is important and should be done 
                    // in the same order as in the the 'domainEvents' enumerable.
                    // They cannot be published in parallel.
                    foreach (var domainEvent in publicEvents)
                    {
                        var message = ToEventMessage(domainEvent);
                        await sender.SendMessageAsync(message);
                    }
                } 
            }
        }

        protected virtual EventMessage ToEventMessage(DomainEvent domainEvent)
        {
            var serializedEvent = JsonConvert.SerializeObject(domainEvent, _serializerSettings);

            var result = new EventMessage
            {
                Topic = domainEvent.GetType().FullName,
                CorrelationId = Guid.NewGuid(),
                Timestamp = DateTime.Now.Ticks,
                EventType = domainEvent.GetType().Name,
                Body = Encoding.Unicode.GetBytes(serializedEvent),
            };

            return result;
        }
    }
}
