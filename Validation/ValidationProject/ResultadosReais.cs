using Comuns.Classes;
using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidationProject
{
    public class ResultadosReais : IPredictionResult, IResult
    {
        public ICollection<Prediction> Predictions { get; set; }
        public bool Success { get => true; set => Success = true; }
        public Exception? Exception { get => null; set => Exception = null; }
        public ICollection<string> Messages { get => null; set => Messages = null; }
    }
}
