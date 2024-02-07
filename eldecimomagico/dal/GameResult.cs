using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class GameResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [BsonElement("gameId")]
        [JsonPropertyName("gameId")]
        public string GameId { get; set; }

        [BsonElement("gameAwardId")]
        [JsonPropertyName("gameAwardId")]
        public string GameAwardId { get; set; }

        [BsonElement("starsBet")]
        [JsonPropertyName("starsBet")]
        public int StarsBet { get; set; }

        [BsonElement("result")]
        [JsonPropertyName("result")]
        public int Result { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }
    }
}