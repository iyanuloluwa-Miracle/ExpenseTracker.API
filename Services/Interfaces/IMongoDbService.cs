// File: Services/Interfaces/IMongoDbService.cs
using AuthSystem.Models;
using MongoDB.Driver;

namespace AuthSystem.Services.Interfaces
{
    public interface IMongoDbService
    {
        IMongoCollection<User> Users { get; }
        IMongoCollection<Token> Tokens { get; }
    }
}