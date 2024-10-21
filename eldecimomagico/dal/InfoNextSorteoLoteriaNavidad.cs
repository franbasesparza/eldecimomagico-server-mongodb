using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    //IMPORTANTE: cuando se pase del estado 1 a estado 2, hay que actualizar la fecha del próximo sorteo.
    public class InfoNextSorteoLoteriaNavidad
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("dateNextSorteo")]
        [JsonPropertyName("dateNextSorteo")]
        public DateTime DateNextSorteo { get; set; }

        [BsonElement("status")]
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}


