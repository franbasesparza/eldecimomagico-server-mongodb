using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{

    public enum PremioLoteriaNavidadType
    {
        PRIMER_PREMIO,
        SEGUNDO_PREMIO,
        TERCER_PREMIO,
        CUARTO_PREMIO_1,
        CUARTO_PREMIO_2,
        QUINTO_PREMIO_1,
        QUINTO_PREMIO_2,
        QUINTO_PREMIO_3,
        QUINTO_PREMIO_4,
        QUINTO_PREMIO_5,
        QUINTO_PREMIO_6,
        QUINTO_PREMIO_7,
        QUINTO_PREMIO_8,
        PEDREA,
        UNKNOWN
    }

    public enum PremioLoteriaNavidadAmount
    {
        PRIMER_PREMIO = 400000,
        SEGUNDO_PREMIO = 125000,
        TERCER_PREMIO = 50000,
        CUARTO_PREMIO = 20000,
        QUINTO_PREMIO = 6000,
        PEDREA = 100,
        UNKNOWN = 0
    }
    public class PremioLoteriaNavidad
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("year")]
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [BsonElement("number")]
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [BsonElement("type")]
        [JsonPropertyName("type")]
        public PremioLoteriaNavidadType Type { get; set; }

        [BsonElement("amount")]
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [BsonElement("updatedOn")]
        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}


