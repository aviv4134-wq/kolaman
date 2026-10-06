using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NotificationGate.Models
{
    public class Alert
    {
        [BsonId]
        [JsonPropertyName("alert_id")]
        public string Alert_id { get; set; } = string.Empty;
        
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("title")]

        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Contnet { get; set; } = string.Empty;

        [JsonPropertyName("priority")]

        public string Priority { get; set; } = string.Empty;

        [JsonPropertyName("classification")]

        public string Classification { get; set; } = string.Empty;

        [JsonPropertyName("lat")]

        public double Lat { get; set; }

        [JsonPropertyName("lon")]

        public double Lon { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("status")]

        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("geographic_command")]

        public string Geographic_location_command { get; set; }
    }
}
