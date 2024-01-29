using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class GameAward
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("gameAwardTypeId")]
        [JsonPropertyName("gameAwardTypeId")]
        public string GameAwardTypeId { get; set; }

        [BsonElement("quantity")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}

