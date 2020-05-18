using DDD.Core.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDD.Core.Application
{
    public static class EventListenerApplicationBuilderExtensions
    {
        public static void UseEventListeners<Tconnection>(this IApplicationBuilder app, 
                                             string queueName, Action<IEventListenerBuilder> addEventHandlers)
        {
            var eventListenerBuilder = new EventListenerBuilder<Tconnection>(app.ApplicationServices);

            addEventHandlers(eventListenerBuilder);

            var eventListener = eventListenerBuilder.CreateEventListener();
            eventListener.StartListening(queueName);
        }
    }
}
