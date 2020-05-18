using Microsoft.Extensions.DependencyInjection;
using Minor.Miffy;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application
{
    public class EventListenerBuilder<TConnection> : IEventListenerBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<string, IEventDispatcher> _dispatchers;

        public EventListenerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dispatchers = new Dictionary<string, IEventDispatcher>();
        }

        public void AddEventHandler<TEvent, THandler>(string topicFilter)
            where THandler : IEventHandler<TEvent>
        {
            var dispatcher = new EventDispatcher<TEvent, THandler>(_serviceProvider);
            _dispatchers.Add(topicFilter, dispatcher);
        }

        public IEventListener CreateEventListener()
        {
            var busContext = _serviceProvider.GetService<IBusContext<TConnection>>();

            var eventListener = new EventListener<TConnection>(busContext, _dispatchers);
            return eventListener;
        }
    }
}
