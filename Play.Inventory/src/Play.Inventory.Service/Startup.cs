using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;

namespace Play.Inventory.Service
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

            Random jitterer = new Random();
            services.AddMongo();
            services.AddMongoRepo<InventoryItem>("inventoryitems");
            services.AddMongoRepo<CatalogItem>("catalogitems");
            services.AddMasstransitWithRabbitMq();

            //client set up and adding co
            services.AddHttpClient<CatalogClient>(Client =>
            {

                Client.BaseAddress = new Uri("https://localhost:5001");
            }

            ).AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutException>().WaitAndRetryAsync(5,
            retryattempt => TimeSpan.FromSeconds(Math.Pow(2, retryattempt))
            + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)) // to have retry after random ms after http call failure
            ))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutException>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(15)))
            //circuit breaker - prevents service from performing operation that is likely to fail

            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1)); // timeout after 1 sec
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Inventory.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Inventory.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
