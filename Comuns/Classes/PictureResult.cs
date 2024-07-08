using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    internal class PictureResult
    {
        public uint Second { get; set; }
        public ICollection<IPredictionData> Detections { get; set; }
    }
}
