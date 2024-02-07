using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class GameAwardSet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("playedCount")]
        [JsonPropertyName("playedCount")]
        public int PlayedCount { get; set; }

        [BsonElement("maxRandomNumber")]
        [JsonPropertyName("maxRandomNumber")]
        public int MaxRandomNumber { get; set; }

        [BsonElement("gameAwardIds")]
        [JsonPropertyName("gameAwardIds")]
        public List<string> GameAwardIds { get; set; }

        [BsonElement("gameAwardProbabilities")]
        [JsonPropertyName("gameAwardProbabilities")]
        public List<int> GameAwardFavorableOutcomes { get; set; }
    }
}
