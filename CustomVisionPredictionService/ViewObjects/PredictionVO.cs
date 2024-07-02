using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionPredictionService.ViewObjects
{
    public class PredictionVO
    {
        public double Probability { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
        public BoundingBoxVO BoundingBox { get; set; }
    }
}
