using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Interfaces
{
    public interface IResult
    {
        bool Success { get; set; }
        Exception? Exception { get; set; }
        ICollection<string> Messages { get; set; }
    }
}
