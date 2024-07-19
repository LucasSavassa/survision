using Comuns.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Interfaces
{
    public interface IPredictionResult : IResult
    {
        ICollection<Prediction> Predictions { get; set; }
    }
}
