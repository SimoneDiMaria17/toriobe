using MongoDB.Bson;
using MongoDB.Driver;

namespace backtorio.service;

public class MongoDbService
{
    private readonly IConfiguration _config;
    private static IMongoDatabase? _database;
    private const string connectionUri = "mongodb+srv://python:ciao@censitorio.svphwfh.mongodb.net/?retryWrites=true&w=majority&appName=Censitorio";
    public MongoDbService(IConfiguration config)
    {
        _config = config;

        var settings = MongoClientSettings.FromConnectionString(_config.GetConnectionString("Db"));
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        var client = new MongoClient(settings);
        try
        {
            _database = client.GetDatabase("Censitorio");
            Console.Clear();
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            Console.WriteLine();
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }
    }
    
    public static IMongoDatabase? Db => _database;
}
/*
 * 
using MongoDB.Driver;
using MongoDB.Bson;

const string connectionUri = "mongodb+srv://<db_username>:<db_password>@censitorio.svphwfh.mongodb.net/?retryWrites=true&w=majority&appName=Censitorio";

var settings = MongoClientSettings.FromConnectionString(connectionUri);

// Set the ServerApi field of the settings object to set the version of the Stable API on the client
settings.ServerApi = new ServerApi(ServerApiVersion.V1);

// Create a new client and connect to the server
var client = new MongoClient(settings);

// Send a ping to confirm a successful connection
try {
  var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
  Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
} catch (Exception ex) {
  Console.WriteLine(ex);
}

 */