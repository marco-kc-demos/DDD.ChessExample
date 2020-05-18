using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Minor.Miffy;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DDD.Core.Application
{
    internal class EventDispatcher<TEvent, THandler> : IEventDispatcher
        where THandler : IEventHandler<TEvent>
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch(EventMessage message)
        {
            // create handler
            THandler handler = ActivatorUtilities.CreateInstance<THandler>(_serviceProvider);

            // deserialize domain event
            string body = Encoding.Unicode.GetString(message.Body);
            TEvent domainEvent = JsonConvert.DeserializeObject<TEvent>(body);

            // call handler
            handler.HandleEvent(domainEvent);
        }
    }
}