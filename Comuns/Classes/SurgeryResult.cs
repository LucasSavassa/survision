using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    internal class SurgeryResult
    {
        public Surgery Surgery { get; set; }
        public ICollection<PictureResult> Timeline { get; set; }
    }
}
