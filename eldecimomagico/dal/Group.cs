using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text;
using System.Linq;

namespace eldecimomagico.dal
{
    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [BsonElement("groupTypeId")]
        [JsonPropertyName("groupTypeId")]
        public string GroupTypeId { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [BsonElement("groupPhotoUrl")]
        [JsonPropertyName("groupPhotoUrl")]
        public string? GroupPhotoUrl { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"Group(Id={Id}, Name={Name}, Type={GroupTypeId}, CreatedOn={CreatedOn}");
            /*
            if (GroupMemberIds != null && GroupMemberIds.Any())
            {
                sb.Append(", GroupMembers=[");

                foreach (var member in GroupMemberIds)
                {
                    sb.Append(member.ToString() + ", ");
                }
                sb.Length -= 2; // Remove the trailing comma and space
                sb.Append("]");
            }
            */

            return sb.ToString();
        }
    }
}

