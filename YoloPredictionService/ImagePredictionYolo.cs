using Comuns.Classes;
using System.Drawing;
using SkiaSharp;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using YoloDotNet;
using System.Drawing.Imaging;
using YoloDotNet.Extensions;
using Comuns.Interfaces;

namespace YoloPredictionService
{
    public class ImagePredictionYolo : IPredictionService
    {
        private string _modelPath;
        private YoloOptions _yoloOptions;

        public ImagePredictionYolo()
        {
            _modelPath = Path.Combine(Environment.CurrentDirectory, "HelperFiles", "YoloModel.onnx");
            _yoloOptions = new YoloOptions()
            {
                OnnxModel = _modelPath,
                ModelType = ModelType.ObjectDetection,
                Cuda = false,
            };
        }

        public IPredictionResult GetImageResults(Bitmap image, double threshold = 0)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Jpeg);
                    ms.Seek(0, SeekOrigin.Begin);

                    using (SKImage skImage = SKImage.FromEncodedData(ms))
                    using (var yolo = new Yolo(_yoloOptions))
                    {
                        var results = yolo.RunObjectDetection(skImage, threshold / 100);

                        return BuildPredictionYoloVO(results, image.Height, image.Width);

                    }
                }
            }
            catch (Exception ex)
            {
                return new PredictionYoloVO()
                {
                    Success = false,
                    Exception = ex
                };
            }
        }

        private PredictionYoloVO BuildPredictionYoloVO(List<ObjectDetection> output, double imageHeight, double imageWidth)
        {
            var predictionYoloVO = new PredictionYoloVO
            {
                Success = true,
                Exception = null,
                Predictions = output
                    .Select(o => new Prediction(
                        name: o.Label.Name,
                        probability: o.Confidence,
                        left: o.BoundingBox.Left > 0 ? (o.BoundingBox.Left / imageWidth) : 0,
                        top: o.BoundingBox.Top > 0 ? (o.BoundingBox.Top / imageHeight) : 0,
                        height: o.BoundingBox.Height / imageHeight,
                        width: o.BoundingBox.Width / imageWidth
                    ))
                    .ToList(),
            };

            return predictionYoloVO;
        }
    }
}

