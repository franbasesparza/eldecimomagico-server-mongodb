using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [BsonElement("surame")]
        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [BsonElement("nickname")]
        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }

        [BsonElement("phone")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [BsonElement("createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [BsonElement("isRegistered")]
        [JsonPropertyName("isRegistered")]
        public bool IsRegistered { get; set; }

        [BsonElement("backgroundImage")]
        [JsonPropertyName("backgroundImage")]
        public string? BackgroundImage { get; set; }
    }
}


