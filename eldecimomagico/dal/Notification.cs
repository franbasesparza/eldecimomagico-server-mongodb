using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eldecimomagico.dal
{
    public class Notification
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}

