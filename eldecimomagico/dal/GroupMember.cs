using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class GroupMember
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("phone")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [BsonElement("isAdmin")]
        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }

        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
       
        [BsonElement("groupId")]
        [JsonPropertyName("groupId")]
        public string GroupId { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"GroupMember(Id={Id}, Phone={Phone}, IsAdmin={IsAdmin}, UserId={UserId}, GroupId={GroupId})");
            return sb.ToString();
        }
    }
}