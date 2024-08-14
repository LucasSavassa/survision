using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVisionPredictionService
{
    public struct ImageSettings
    {
        public const int ImageHeight = 320;
        public const int ImageWidth = 320;       
    }

    public class ImageInput
    {
        [ImageType(ImageSettings.ImageHeight, ImageSettings.ImageWidth)]
        public MLImage Image { get; set; }
    }
}
