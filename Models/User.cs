// File: Models/User.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthSystem.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("passwordSalt")]
        public string PasswordSalt { get; set; } = string.Empty;

        [BsonElement("isEmailVerified")]
        public bool IsEmailVerified { get; set; }

        [BsonElement("emailVerificationToken")]
        public string? EmailVerificationToken { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("lastLogin")]
        public DateTime? LastLogin { get; set; }
    }
}
