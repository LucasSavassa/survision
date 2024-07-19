using Comuns.Classes;
using Comuns.Interfaces;
using CustomVisionPredictionService.ViewObjects;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http.Headers;

namespace CustomVisionPredictionService
{
    public class ImagePredictionCustomVision : IPredictionService
    {
        private string _endpoint;
        private string _predictionKey;
        private const int MaxSizeImage = 3000000;
        private const double MinPercertageResize = 0.99;
        private static readonly HttpClient client = new HttpClient();

        public ImagePredictionCustomVision()
        {
            _endpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/78aafb04-a169-41e5-a70b-219d0b8f36e4/detect/iterations/Iteration2/image";
            _predictionKey = "2fa0151d6b8149bfb94dbf01d2bf7b00";
        }

        public async Task<IPredictionResult> GetImageResults(Bitmap image, double threshold = 0, double iof = 0, bool resizeImage = false)
        {
            try
            {
                byte[] imageBytes = GetResizedImageBytes(image, resizeImage);
                var imageResultString = await SendImageToCustomVisionPrediction(imageBytes);

                RootVO rootObject = JsonConvert.DeserializeObject<RootVO>(imageResultString);
                return BuildPredictionCustomVisionVO(rootObject, threshold);

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

        private byte[] GetResizedImageBytes(Bitmap image, bool resize)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);

                while (resize && ms.Length > MaxSizeImage)
                {
                    int newWidth = (int)(image.Width * MinPercertageResize);
                    int newHeight = (int)(image.Height * MinPercertageResize);

                    ms.Seek(0, SeekOrigin.Begin);
                    using (Bitmap Resizedimage = new (image, new Size(newWidth, newHeight)))
                    { 
                        ms.SetLength(0);
                        Resizedimage.Save(ms, ImageFormat.Jpeg);
                    }
                }
                return ms.ToArray();
            }
        }

        private async Task<string> SendImageToCustomVisionPrediction(byte[] imageBytes)
        {
            var content = new ByteArrayContent(imageBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var request = new HttpRequestMessage(HttpMethod.Post, _endpoint)
            {
                Content = content
            };

            request.Headers.Add("Prediction-Key", _predictionKey);
            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private IPredictionResult BuildPredictionCustomVisionVO(RootVO root, double threshold)
        {
            var predictionCustomVisionVO = new PredictionCustomVisionVO
            {
                Success = true,
                Exception = null,
                Predictions = root.Predictions
                    .Where(p => p.Probability > (threshold / 100))
                    .Select(p => new Prediction
                        (                                                  
                            name: p.TagName,
                            probability: p.Probability,
                            left: p.BoundingBox.Left,
                            top: p.BoundingBox.Top,
                            height: p.BoundingBox.Height,
                            width: p.BoundingBox.Width
                        )
                    )
                    .ToList()
            };

            return predictionCustomVisionVO;
        }


    }
}

