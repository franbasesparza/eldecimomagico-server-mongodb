using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class PremioLoteriaNavidad
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("year")]
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [BsonElement("number")]
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [BsonElement("amount")]
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }
}


