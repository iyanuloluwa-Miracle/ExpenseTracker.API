// File: Models/Token.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Models
{
    public class Token
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("tokenValue")]
        public string TokenValue { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = "reset"; // reset or verify

        [BsonElement("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [BsonElement("isUsed")]
        public bool IsUsed { get; set; }
    }
}