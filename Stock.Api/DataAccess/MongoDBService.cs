using MongoDB.Driver;

namespace Stock.Api.DataAccess;

public class MongoDBService
{
    public IMongoDatabase _Database { get; set; }
    public MongoDBService(IConfiguration _configuration)
    {
        MongoClient mongoClient = new MongoClient(_configuration.GetConnectionString("MongoDb"));
        _Database = mongoClient.GetDatabase("StockDb");
    }

    public IMongoCollection<T> GetCollection<T>() => _Database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());

}