using Comuns.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    internal class Surgery
    {
        public int Id { get; set; }
        public SurgeryType Type { get; set; }
        public uint Seconds { get; set; }
        public uint Shots { get; set; }
        public DateTime Start { get; set; }
    }
}
