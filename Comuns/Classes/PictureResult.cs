using Comuns.Interfaces;
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
        public ICollection<Prediction> Detections { get; set; }

        [JsonPropertyName("hash")]
        public int Hash { get { return GetSortingAgnosticHash(); } }

        public PictureResult()
        {
            Detections = [];
        }

        public int GetSortingAgnosticHash()
        {
            IEnumerable<(string, int)> groups = Detections
                .GroupBy(detection => detection.Name)
                .Select(group => (group.First().Name, group.Count()));

            int code = 2147483647;
            foreach ((string name, int count) in groups)
            {
                code ^= name.GetHashCode() + count;
            }
            return code;
        }
    }
}
