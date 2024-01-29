using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace eldecimomagico.dal
{
    public class GroupMemberParticipation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("groupId")]
        [JsonPropertyName("groupId")]
        public string GroupId { get; set; }

        [BsonElement("groupMemberId")]
        [JsonPropertyName("groupMemberId")]
        public string GroupMemberId { get; set; }
        
        [BsonElement("number")]
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [BsonElement("year")]
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [BsonElement("amount")]
        [JsonPropertyName("amount")]
        public float Amount { get; set; }

        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            return $"GroupMemberParticipation(Id={Id}, GroupMemberId={GroupMemberId}, Number={Number}, Year={Year}, Amount={Amount}, Description={Description}, CreatedOn={CreatedOn})";
        }
    }
}