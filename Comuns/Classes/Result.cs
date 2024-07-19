using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Classes
{
    public class Result : IResult
    {
        public bool Success { get; set; }
        public Exception? Exception { get; set; }
        public ICollection<string> Messages { get; set; } = [];

        public static Result Successfull => new Result { Success = true };
    }
}
