using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;


namespace Play.Common.Settings
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {

        // private const string collectionName = "Items"; // group of objects in DB its like table

        private readonly MongoCollectionBase<T> dbCollection; // represent a Type collection

        private readonly FilterDefinitionBuilder<T> filterbuilder = Builders<T>.Filter; // to query info from Mongo DB you need to create filter 

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            //connect to mongodb

            /*   var mongoClient = new MongoClient("mongodb://localhost:27017"); // to connect to DB with connection string
             // var database = mongoClient.GetDatabase("Catalog"); // Database object */
            // dbcollection object for doing operation
            dbCollection = (MongoCollectionBase<T>)database.GetCollection<T>(collectionName);

        }
        // client is expecting readonly collection of items so return type is Task of IReadonlyCollection
        // FilterDefinitions are a fast and intuitive way to create filters. 
        //They require less processing then using expressions and offer a lot of methods for you to use.
        //PS C:\Users\abhis\Documents\Development\DotNet\MicroServices> 
        // docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db

        public async Task<IReadOnlyCollection<T>> GetAllAsync()

        {

            // to get all items
            // find ,replaceoneasync , removeoneasync method need filter parameter to 
            return await dbCollection.Find(filterbuilder.Empty).ToListAsync();

        }

        public async Task<T> GetItemAsync(Guid id)
        {
            // create equality filter for finding record associated with ID
            FilterDefinition<T> filter = filterbuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity) // not returning anything to  client no type parameter for Task
        {

            if (entity == null)
            {

                throw new ArgumentNullException(nameof(entity));

            }
            await dbCollection.InsertOneAsync(entity);

        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {

                throw new ArgumentNullException(nameof(entity));

            }

            FilterDefinition<T> filter = filterbuilder.Eq(existingentity => existingentity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);


        }

        public async Task RemoveAsync(Guid id)
        {

            FilterDefinition<T> filter = filterbuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

        async Task<IReadOnlyCollection<T>> IRepository<T>.GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }

        async Task<T> IRepository<T>.GetItemAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();

        }
    }
}