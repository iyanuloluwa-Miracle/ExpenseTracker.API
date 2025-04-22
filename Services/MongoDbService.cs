// File: Services/MongoDbService.cs
using Server.Configuration;
using Server.Models;
using Server.Services.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Server.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Token> Tokens => _database.GetCollection<Token>("Tokens");
    }
}