using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Interfaces
{
    public interface IPredictionResult<T> where T : IPredictionData
    {
        bool IsSuccess { get; set; }
        Exception Exception { get; set; }
        List<T> PredictionDatas { get; set; }
    }
}
