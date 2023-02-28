using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catlog.Service.Entities;

using Play.Common.MongoDB;
using Play.Common.Settings;

namespace Play.Catlog.Service
{
    public class Startup
    {

        private ServiceSettings serviceSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*  
                        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
                        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
                        /*So with that we are deserializing the value

                    of ServiceSettings that has already been loaded

                    into .NET conversion system

                    into this ServiceSettings variable here.

                        serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                        Console.WriteLine(serviceSettings.ToString());

                        /*  Configuration.GetSection(nameof(serviceSettings)).Get<ServiceSettings>();
                        Attempts to bind (deserialize) the configuration instance to a new instance of type T. If this configuration section has a value, that will be used. Otherwise binding by matching property names against configuration keys recursively.
                        Returns:      The new instance of T if successful, default(T) otherwise. 


                        services.AddSingleton<IMongoDatabase>((ServiceProvider) =>
                        //Adds a singleton service of the type specified in IMongoDatabase with a 
                        //factory specified in implementationFactory to the specified IServiceCollection
                        //constructing IMongoDatabse and registering it with service container.

                        {

                            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                            return mongoClient.GetDatabase(serviceSettings.ServiceName);
                        }); 

            services.AddSingleton<IRepository<Item>>(ServiceProvider =>
            {

                var database = ServiceProvider.GetService<IMongoDatabase>();

                return new MongoRepository<Item>(database, "Items");



            });*/


            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddMongo();
            services.AddMongoRepo<Item>("Items");


            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;

            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Catlog.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catlog.Service v1"));
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
