using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationProject
{
    public class ObjectResults
    {
        public string ObjectName { get; set; }
        public int VP { get; set; } = 0;
        public int FP { get; set; } = 0;
        public int FN { get; set; } = 0;
    }
}
