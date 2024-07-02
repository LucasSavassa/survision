using Comuns.Classes;
using CustomVisionPredictionService.ViewObjects;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace CustomVisionPredictionService
{
    public class ImagePrediction
    {
        private string _endpoint;
        private string _predictionKey;
        private const int MaxSizeImage = 3000000;
        private const double MinPercertageResize = 0.99;
        private static readonly HttpClient client = new HttpClient();

        public ImagePrediction()
        {
            _endpoint = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/78aafb04-a169-41e5-a70b-219d0b8f36e4/detect/iterations/Iteration2/image";
            _predictionKey = "2fa0151d6b8149bfb94dbf01d2bf7b00";
        }

        public async Task<PredictionCustomVisionVO> GetImageResults(Stream image, double threshold = 0, bool resizeImage = false)
        {
            try
            {
                byte[] imageBytes = await GetResizedImageBytes(image, resizeImage);
                var imageResultString = await SendImageToCustomVisionPrediction(imageBytes);
                RootVO rootObject = JsonConvert.DeserializeObject<RootVO>(imageResultString);
                return BuildPredictionCustomVisionVO(rootObject, threshold);
            }
            catch (Exception ex)
            {
                return new PredictionCustomVisionVO()
                {
                    IsSuccess = false,
                    Exception = ex
                };
            }
            
        }

        private async Task<byte[]> GetResizedImageBytes(Stream imageStream, bool resize)
        {
            using (var ms = new MemoryStream())
            {
                if (!resize || imageStream.Length <= MaxSizeImage)                
                    await imageStream.CopyToAsync(ms);
                    
                while (resize && imageStream.Length > MaxSizeImage)
                {
                    using (Image image = Image.Load(imageStream))
                    {
                        int newWidth = (int)(image.Width * MinPercertageResize);
                        int newHeight = (int)(image.Height * MinPercertageResize);

                        image.Mutate(x => x.Resize(newWidth, newHeight));
                        await image.SaveAsync(ms, new JpegEncoder());
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
            return await response.Content.ReadAsStringAsync();
        }

        public static PredictionCustomVisionVO BuildPredictionCustomVisionVO(RootVO root, double threshold)
        {
            var predictionCustomVisionVO = new PredictionCustomVisionVO
            {
                IsSuccess = true, 
                Exception = null, 
                PredictionDatas = root.Predictions.Where(p => p.Probability > (threshold / 100))
                                                  .Select(p => new PredictionDataCustomVisionVO
                {
                    Name = p.TagName,
                    Probability = p.Probability,
                    Left = p.BoundingBox.Left,
                    Top = p.BoundingBox.Top,
                    Height = p.BoundingBox.Height,
                    Width = p.BoundingBox.Width
                }).ToList()
            };

            return predictionCustomVisionVO;
        }


    }
}

