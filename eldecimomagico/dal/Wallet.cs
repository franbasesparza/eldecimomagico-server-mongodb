using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class Wallet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [BsonElement("balance")]
        [JsonPropertyName("balance")]
        public float Balance { get; set; }

        [BsonElement("stars")]
        [JsonPropertyName("stars")]
        public int Stars { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        public string ToString(){
            return $"Wallet (Id={Id}, UserId={UserId}, Balance={Balance}, Stars={Stars}";
        }

    }
}