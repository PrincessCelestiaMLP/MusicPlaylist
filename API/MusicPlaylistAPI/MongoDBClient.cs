using MongoDB.Driver;

namespace MusicPlaylistAPI;

public class MongoDBClient
{
    private static IMongoDatabase _database;
    private static MongoDBClient _instance;

    public static MongoDBClient Instance
    {
        get => _instance ?? new MongoDBClient();
    }

    private MongoDBClient()
    {
        var connectionString = "mongodb+srv://musicUser:dX1CUqEwz83jBCZ6@musiccluster.strlxso.mongodb.net/?appName=MusicCluster";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("MusicPlaylistDB");
        _instance = this;
    }

    public IMongoCollection<T> GetCollection<T>(string name) => _database.GetCollection<T>(name);
}
