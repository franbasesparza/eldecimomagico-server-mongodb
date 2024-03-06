using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace eldecimomagico.dal
{   
    public class InAppPurchase
    {
        public static Dictionary<string, int> starMap = new Dictionary<string, int>
        {
            { "recharge_100_stars", 100 },
            { "recharge_250_stars", 250 },
            { "recharge_250_stars", 500 }
        };
       
        public static string STORE_SUBSCRIPTION_BASIC = "subscription_basic";
        public static string STORE_SUBSCRIPTION_PREMIUM = "subscription_premium";

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [BsonElement("productId")]
        [JsonPropertyName("productId")]
        public string ProductId { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }
    }
}