using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Interfaces
{
    public interface IPredictionService
    {
        public IPredictionResult GetImageResults(Bitmap image, double threshold = 0);
    }
}
