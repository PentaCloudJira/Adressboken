using MongoDB.Driver;

namespace Adressboken.Data
{
    public class MongoDBHandler
    {
        private readonly IMongoDatabase _database;

        public MongoDBHandler(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
