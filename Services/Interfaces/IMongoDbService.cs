// File: Services/Interfaces/IMongoDbService.cs
using Server.Models;
using MongoDB.Driver;

namespace Server.Services.Interfaces
{
    public interface IMongoDbService
    {



        IMongoCollection<Expense> Expenses { get; }

        
        IMongoCollection<User> Users { get; }
        IMongoCollection<Token> Tokens { get; }

        IMongoCollection<Category> Categories { get; }
    }
}