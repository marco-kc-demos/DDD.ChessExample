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
    public static class EventStoreServiceCollectionExtensions
    {
        public static void AddEventStoreContext<TEventStoreContext>(this IServiceCollection services,
                Action<DbContextOptionsBuilder<TEventStoreContext>> optionsConfiguration)
            where TEventStoreContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TEventStoreContext>();
            optionsConfiguration(builder);
            var options = builder.Options;

            services.AddSingleton(options);
        }
    }
}
