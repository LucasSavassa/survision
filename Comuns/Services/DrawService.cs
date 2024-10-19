using Comuns.Classes;
using Comuns.Enums;
using Comuns.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comuns.Services
{
    public static class DrawService
    {
        public static void DrawDetectedObjects(Bitmap image, IPredictionResult predictionResult, double inferiorThreshold)
        {
            foreach (Prediction prediction in predictionResult.Predictions)
            {
                float left = (float)prediction.Left * image.Width;
                float top = (float)prediction.Top * image.Height;
                float width = (float)prediction.Width * image.Width;
                float height = (float)prediction.Height * image.Height;
                string name = prediction.Name;
                double probability = prediction.Probability;

                RectangleF rectangleF = new(left, top, width, height);
                DrawObject(image, rectangleF, name, probability, inferiorThreshold);
            }
        }

        public static void DrawObject(Bitmap bitmap, RectangleF rectangleF, string objectName, double probability, double inferiorThreshold)
        {
            bool isDetected = probability >= inferiorThreshold/100;
            Color lineColor = isDetected ? Color.Yellow : Color.Red;
            Color fontColor = isDetected ? Color.Yellow : Color.Red;
            string name = isDetected ? objectName : "Desconhecido";
            string text = $"{name} - {probability * 100:0.00}%";

            using (Graphics g = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(lineColor, 2))
            using (Font font = new Font("Arial", 16, FontStyle.Bold))
            using (Brush brush = new SolidBrush(fontColor))
            {
                g.DrawRectangle(pen, rectangleF);
                g.DrawString(text, font, brush, new PointF(rectangleF.Left, rectangleF.Top - 30));
            }
        }
    }
}
