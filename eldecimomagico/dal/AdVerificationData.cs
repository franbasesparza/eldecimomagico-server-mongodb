using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace eldecimomagico.dal
{
    public class AdVerificationData
    {
        [JsonProperty("ad_network")]
        public string AdNetwork { get; set; }

        [JsonProperty("ad_unit")]
        public string AdUnit { get; set; }

        [JsonProperty("custom_data")]
        public string CustomData { get; set; }

        [JsonProperty("key_id")]
        public string KeyId { get; set; }

        [JsonProperty("reward_amount")]
        public int RewardAmount { get; set; }

        [JsonProperty("reward_item")]
        public string RewardItem { get; set; }

        [JsonProperty("firma")]
        public string Firma { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        public override string ToString()
        {
            return $"AdVerificationData (ad_network={AdNetwork}, ad_unit={AdUnit}, custom_data={CustomData}, key_id={KeyId}, reward_amount={RewardAmount}, reward_item={RewardItem}, firma={Firma}, timestamp={Timestamp}, transaction_id={TransactionId}, user_id={UserId})";
        }

    }
}
