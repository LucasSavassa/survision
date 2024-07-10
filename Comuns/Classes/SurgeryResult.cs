using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public class SurgeryResult
    {
        [JsonPropertyName("surgery")]
        public Surgery Surgery { get; set; }

        [JsonPropertyName("timeline")]
        public ICollection<PictureResult> Timeline { get; set; }

        public SurgeryResult()
        {
            Surgery = new Surgery();
            Timeline = [];
        }
    }
}
