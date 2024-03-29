using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class NotificationToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userPhoneNumber")]
        [JsonPropertyName("userPhoneNumber")]
        public string UserPhoneNumber { get; set; }

        [BsonElement("token")]
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}


