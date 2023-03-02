using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMasstransitWithRabbitMq(this IServiceCollection services)
        {
            // add masstransit service
            services.AddMassTransit(x =>

           {

               x.AddConsumers(Assembly.GetEntryAssembly());
               //configure RabbitMQ
               x.UsingRabbitMq((context, configurator) =>
               {

                   var configuration = context.GetService<IConfiguration>();
                   ServiceSettings serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                   var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();

                   configurator.Host(rabbitMQSettings.Host);

                   configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

               });
           });
            services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(30);
    options.StopTimeout = TimeSpan.FromMinutes(1);
});

            return services;
        }

    }
}