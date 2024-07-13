using Comuns.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Interfaces
{
    public interface IPredictionResult
    {
        bool IsSuccess { get; set; }
        Exception Exception { get; set; }
        List<PredictionData> PredictionDatas { get; set; }
    }
}
