using DDD.Core.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minor.Miffy;
using Minor.Miffy.RabbitMQBus;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDD.Core.Application
{
    public static class BusContextServiceCollectionExtensions
    {
        public static void AddRabbitMQBusContext(this IServiceCollection services,
                                                   Action<RabbitMQBusContextBuilder> busContextConfiguration)
        {
            services.AddTransient<IEventPublisher, EventPublisher<IConnection>>();

            var builder = new RabbitMQBusContextBuilder();
            busContextConfiguration(builder);
            IBusContext<IConnection> context = builder.CreateContext();

            services.AddSingleton(context);
        }

        public static void AddBusContext<TConnection>(this IServiceCollection services,
                                                      IBusContext<TConnection> busContext)
        {
            services.AddTransient<IEventPublisher, EventPublisher<TConnection>>();

            services.AddSingleton(busContext);
        }
    }
}
