using Comuns.Classes;
using Comuns.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.ML;
using Microsoft.ML.Transforms.Image;
using Microsoft.ML.Data;

namespace CustomVisionPredictionService
{
    public class ImagePredictionCustomVision : IPredictionService
    {
        private string _modelPath;
        private string _outputColumnNameOnnx;
        private string[] _outputColumnsNamesOnnx;
        private string[] _inputColumnNameOnnx;
        private string[] _labels;

        public ImagePredictionCustomVision()
        {
            _modelPath = Path.Combine(Environment.CurrentDirectory, "HelperFiles", "AzureModel.onnx");
            _outputColumnNameOnnx = "image_tensor";
            _outputColumnsNamesOnnx = new string[] { "detected_boxes", "detected_scores", "detected_classes" };
            _inputColumnNameOnnx = new string[] { "image_tensor" };
            _labels = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "HelperFiles", "labels.txt"));
        }

        public IPredictionResult GetImageResults(Bitmap image, double threshold = 0)
        {
            try
            {

                var context = new MLContext();
                var data = context.Data.LoadFromEnumerable(new List<ImageInput>());

                var pipeline = context.Transforms.ResizeImages(resizing: ImageResizingEstimator.ResizingKind.Fill,
                                                               outputColumnName: _outputColumnNameOnnx,
                                                               imageWidth: ImageSettings.ImageWidth,
                                                               imageHeight: ImageSettings.ImageHeight,
                                                               inputColumnName: nameof(ImageInput.Image))
                                                 .Append(context.Transforms.ExtractPixels(outputColumnName: _outputColumnNameOnnx))
                                                 .Append(context.Transforms.ApplyOnnxModel(outputColumnNames: _outputColumnsNamesOnnx, inputColumnNames: _inputColumnNameOnnx, modelFile: _modelPath));

                using (var model = pipeline.Fit(data))
                using (var predictionEngine = context.Model.CreatePredictionEngine<ImageInput, ImagePredictionResults>(model))
                using (MemoryStream ms = new())
                {
                    image.Save(ms, ImageFormat.Jpeg);
                    ms.Seek(0, SeekOrigin.Begin);
                    var mlImage = MLImage.CreateFromStream(ms);

                    var prediction = predictionEngine.Predict(new ImageInput { Image = mlImage });

                    return BuildPredictionCustomVisionVO(prediction, threshold / 100);
                }
            }
            catch (Exception ex)
            {
                return new PredictionCustomVisionVO()
                {
                    Success = false,
                    Exception = ex
                };
            }

        }

        private IPredictionResult BuildPredictionCustomVisionVO(ImagePredictionResults resultados, double threshold)
        {
            var predictionCustomVisionVO = new PredictionCustomVisionVO
            {
                Success = true,
                Exception = null,
                Predictions = new List<Prediction>()
            };

            int length = resultados?.PredictedLabels?.Length ?? 0;

            for(int i = 0; i < length; i ++)
            {
                string name = _labels[resultados.PredictedLabels[i]];
                double probability = resultados.Scores[i];
                double left = resultados.BoundingBoxes[i * 4];
                double top = resultados.BoundingBoxes[(i * 4) + 1];
                double right = resultados.BoundingBoxes[(i * 4) + 2];
                double bottom = resultados.BoundingBoxes[(i * 4) + 3];
                double height = Math.Abs(top - bottom);
                double width = Math.Abs(right - left);

                var predction = new Prediction(
                    name: name,
                    probability: probability,
                    left: left,
                    top: top,
                    height: height,
                    width: width
                    );

                if(probability >= threshold)
                    predictionCustomVisionVO.Predictions.Add(predction);
            }

            return predictionCustomVisionVO;
        }
    }
}

