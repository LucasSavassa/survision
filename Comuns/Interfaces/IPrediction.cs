using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Interfaces
{
    public interface IPrediction
    {
        string Name { get; }
        double Probability { get; }
        double Left { get; }
        double Top { get; }
        double Height { get; }
        double Width { get; }
    }
}
