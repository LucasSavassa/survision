using Comuns.Classes;
using System.Drawing;
using SkiaSharp;
using YoloDotNet.Enums;
using YoloDotNet.Models;
using YoloDotNet;
using System.Drawing.Imaging;
using Comuns.Abstract;
using YoloDotNet.Extensions;

namespace YoloPredictionService
{
    public class ImagePredictionYolo : ImagePredictionBase
    {
        private string _modelPath;
        private YoloOptions _yoloOptions;

        public ImagePredictionYolo()
        {
            _modelPath = Path.Combine(Environment.CurrentDirectory, "HelperFiles", "model.onnx");
            _yoloOptions = new YoloOptions()
            {
                OnnxModel = _modelPath,
                ModelType = ModelType.ObjectDetection,
                Cuda = false,
            };
        }

        public override async Task<PredictionResultBase> GetImageResults(Bitmap image, double threshold = 0, double iof = 0, bool resizeImage = false)
        {
            return await Task.Run(() =>
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
                            var results = yolo.RunObjectDetection(skImage, threshold/100, 0.1);
                           
                            return BuildPredictionYoloVO(results, image.Height, image.Width);
                          
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new PredictionYoloVO()
                    {
                        IsSuccess = false,
                        Exception = ex
                    };
                }       
            });

        }

        private PredictionYoloVO BuildPredictionYoloVO(List<ObjectDetection> output, double imageHeight, double imageWidth)
        {
            var predictionYoloVO = new PredictionYoloVO
            {
                IsSuccess = true,
                Exception = null,
                PredictionDatas = output.Select(o => new PredictionData
                {
                    Name = o.Label.Name,
                    Probability = o.Confidence,
                    Left = o.BoundingBox.Left > 0 ? (o.BoundingBox.Left / imageWidth) : 0,
                    Top = o.BoundingBox.Top > 0 ? (o.BoundingBox.Top / imageHeight) : 0,
                    Height = o.BoundingBox.Height / imageHeight,
                    Width = o.BoundingBox.Width / imageWidth

                }).ToList(),
            };

            return predictionYoloVO;
        }
    }
}

