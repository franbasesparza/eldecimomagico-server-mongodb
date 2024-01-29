using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("available")]
        [JsonPropertyName("available")]
        public bool Available { get; set; }

        [BsonElement("order")]
        [JsonPropertyName("order")]
        public int Order { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [BsonElement("gameAwardSetId")]
        [JsonPropertyName("gameAwardSetId")]
        public string GameAwardSetId { get; set; }
    }
}

