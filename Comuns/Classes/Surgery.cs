using Comuns.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public class Surgery
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public SurgeryType Type { get; set; }

        [JsonPropertyName("seconds")]
        public uint Seconds { get; set; }

        [JsonPropertyName("shots")]
        public uint Shots { get; set; }

        [JsonPropertyName("start")]
        public DateTime Start { get; set; }
    }
}
