using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
  public class InAppSubscription
  {
    public static Dictionary<string, int> starMap = new Dictionary<string, int>
        {
            { "subscription_basic", 100 },
            { "subscription_premium", 250 },
        };

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("userId")]
    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    [BsonElement("subscriptionTypeId")]
    [JsonPropertyName("subscriptionTypeId")]
    public string SubscriptionTypeId { get; set; }

    [BsonElement("subscriptionToken")]
    [JsonPropertyName("subscriptionToken")]
    public string SubscriptionToken { get; set; }

    [BsonElement("subscriptionState")]
    [JsonPropertyName("subscriptionState")]
    public GoogleInAppSubscriptionState SubscriptionState { get; set; }

    [BsonElement("createdOn")]
    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
  }
}