using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace eldecimomagico.dal
{
    public class UserAdminParticipation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [BsonElement("adminParticipationId")]
        [JsonPropertyName("adminParticipationId")]
        public string AdminParticipationId { get; set; }

        [BsonElement("amount")]
        [JsonPropertyName("amount")]
        public float Amount { get; set; }

        [BsonElement("gameResultId")]
        [JsonPropertyName("gameResultId")]
        public string GameResultId { get; set; }

        [BsonElement("purchaseId")]
        [JsonPropertyName("purchaseId")]
        public string PurchaseId { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }
    }
}