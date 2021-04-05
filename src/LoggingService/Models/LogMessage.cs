using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LoggingService.Models
{
    public class LogMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt => DateTime.Now;
        public string SourceService { get; set; }
        public DateTime TimeStamp { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public Severity Severity { get; set; }

        public string Text { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public LogType LogType { get; set; }
    }
}