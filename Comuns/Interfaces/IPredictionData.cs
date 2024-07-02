using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Interfaces
{
    public interface IPredictionData
    {
        string Name { get; set; }
        double Probability { get; set; }
        double Left {  get; set; }
        double Top { get; set; }
        double Height { get; set; }
        double Width { get; set; }
    }
}
