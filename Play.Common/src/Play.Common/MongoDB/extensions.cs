using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB
{
    public static class extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            services.AddSingleton<IMongoDatabase>((serviceProvider) =>
            //Adds a singleton service of the type specified in IMongoDatabase with a 
            //factory specified in implementationFactory to the specified IServiceCollection
            //constructing IMongoDatabse and registering it with service container.

           {

               var configuration = serviceProvider.GetService<IConfiguration>();
               ServiceSettings serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();


               var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
               var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
               return mongoClient.GetDatabase(serviceSettings.ServiceName);
           });

            return services;
        }

        public static IServiceCollection AddMongoRepo<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {

            services.AddSingleton<IRepository<T>>(ServiceProvider =>
            {

                var database = ServiceProvider.GetService<IMongoDatabase>();

                return new MongoRepository<T>(database, collectionName);



            });

            return services;

        }
    }
}