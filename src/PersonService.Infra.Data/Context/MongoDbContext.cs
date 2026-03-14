using PersonService.Infra.Data.Extensions;
using PersonService.Infra.Data.Indexes;
using PersonService.Infra.Data.Persistence;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace PersonService.Infra.Data.Context
{
    public sealed class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoClient client, IConfiguration configuration)
        {
            MongoDbConventions.RegisterConventions();

            var databaseName = configuration["MongoSettings:DatabaseName"];

            _database = client.GetDatabase(databaseName);

            MongoIndexInitializer.Configure(_database);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(_database.GetCollectionName<T>());
        }
    }
}
