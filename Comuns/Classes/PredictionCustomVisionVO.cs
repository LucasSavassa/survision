using Comuns.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public class PredictionCustomVisionVO : PredictionResultBase
    {
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
        public List<PredictionData> PredictionDatas { get; set; }
       
    }
}
