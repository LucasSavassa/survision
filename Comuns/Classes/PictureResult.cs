using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public class PictureResult
    {
        [JsonPropertyName("second")]
        public uint Second { get; set; }

        [JsonPropertyName("detections")]
        public ICollection<PredictionData> Detections { get; set; }

        public PictureResult()
        {
            Detections = [];
        }
    }
}
