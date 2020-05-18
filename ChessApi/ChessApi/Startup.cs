using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessApi.Application;
using ChessApi.Application.CommandHandlers;
using ChessApi.Application.EventHandlers;
using ChessApi.Application.Repositories;
using ChessApi.Controllers;
using ChessApi.Domain.Aggregates;
using ChessApi.Domain.IncomingDomainEvents;
using DDD.Core.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Minor.Miffy;
using Minor.Miffy.RabbitMQBus;
using RabbitMQ.Client;

namespace ChessApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IMakeMoveCommandHandler, MakeMoveCommandHandler>();
            services.AddTransient<IGameRepository, GameRepository>();
 
            services.AddRabbitMQBusContext(builder => 
                builder.WithExchange("Chess.Exchange"));

            string connectionString =
                    @"Server=localhost,51433;Database=GamesDb;User Id=sa;password=Geheim_101";
            services.AddEventStoreContext<GameEventStoreContext>(optionBuilder =>
                optionBuilder.UseSqlServer(connectionString));

            //services
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseEventListeners<IConnection>(queueName: "Chess.ChessApi", (el) =>
            {
                el.AddEventHandler<GameFormed, GameFormedEventHandler>(topicFilter: "#.GameFormed");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
