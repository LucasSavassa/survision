using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public class PredictionCustomVisionVO : IPredictionResult<PredictionDataCustomVisionVO>
    {
        public bool IsSuccess { get; set; }
        public Exception Exception { get; set; }
        public List<PredictionDataCustomVisionVO> PredictionDatas { get; set; }
    }
}
