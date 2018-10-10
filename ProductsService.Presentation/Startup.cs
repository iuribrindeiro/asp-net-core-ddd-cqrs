using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Bus.EventBus.RabbitAggregate;
using Bus.EventManager;
using Bus.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductsService.Presentation.Infra.Data.ApplicationContext;
using RabbitMQ.Client;

namespace ProductsService.Presentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<ProductsApplicationContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("Products.SQLConnectionString"), sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);

                    });
                });
            services.AddSingleton<IIntegrationEventManager, IntegrationEventManager>();
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = Configuration.GetValue<string>("Products.RabbitMQHost"),
                    UserName = Configuration.GetValue<string>("Products.RabbitMQUserName"),
                    Password = Configuration.GetValue<string>("Products.RabbitMQPassword")
                };

                return new RabbitMqPersistentConnection(connectionFactory, logger, retryCount: 5);
            });
            services.AddSingleton<IEventBus>(sp =>
            {
                var persistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventManager = sp.GetRequiredService<IIntegrationEventManager>();
                var lifeScope = sp.GetRequiredService<ILifetimeScope>();
                var queueName = Configuration.GetValue<string>("Products.RabbitMQQueueName");
                
                return new EventBusRabbitMQ(persistentConnection, logger, eventManager, lifeScope, queueName, retryCount: 5);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
