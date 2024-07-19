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
        public Task<IPredictionResult> GetImageResults(Bitmap image, double threshold = 0, double iof = 0, bool resizeImage = false);
    }
}
