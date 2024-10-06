using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotographService
{
    public record class RecordingMetadata
    {
        public DateTime Start { get; init; }
        public int Shots { get; init; }
        public int Seconds { get; init; }
        public RecordingMetadata(DateTime start, int shots, int seconds)
        {
            Start = start;
            Shots = shots;
            Seconds = seconds;
        }
    }
}
