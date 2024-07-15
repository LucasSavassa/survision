using Comuns.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Abstract
{
    public abstract class PredictionResultBase
    {
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
        public List<PredictionData> PredictionDatas { get; set; }
    }
}
