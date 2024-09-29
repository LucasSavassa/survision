using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using DirectShowLib;
using System.Drawing.Imaging;
using Emgu.CV.Dai;

namespace PhotographService
{
    public class Photographer : IDisposable
    {
        public delegate void ShowCapture(Image image);

        public bool IsCapturing { get; private set; }
        private VideoCapture _captureDevice;
        Mat _frame;
        bool _isDisposing = false;


        public Photographer(string cameraName = "HD Pro Webcam C920", int desiredWidth = 960, int desiredHeight = 720)
        {
            LoadCamera(cameraName, desiredWidth, desiredHeight);
        }

        public bool CapturePhoto(string storagePath, ShowCapture func)
        {
            if (_frame.IsEmpty)
                return false;

            using (Image image = _frame.ToBitmap())
            {
                string filePath = Path.Combine(storagePath, "processing.jpg");
                image.Save(filePath, ImageFormat.Jpeg);
                func(image);
            }

            return true;
        }

        public void StartCapture(string storagePath, int interval, ShowCapture func)
        {
            if (!IsCapturing)
            {
                IsCapturing = true;
                Task.Run(() => CapturePhotos(storagePath, interval, func));
            }
        }

        public void StopCapture()
        {
            IsCapturing = false;
        }

        private void CapturePhotos(string storagePath, int interval, ShowCapture func)
        {
            Stopwatch stopwatch = new Stopwatch();
            int elapsedTime = 0;
            stopwatch.Start();

            while (IsCapturing)
            {
                elapsedTime = (int)stopwatch.Elapsed.TotalSeconds;

                if (CapturePhoto(storagePath, func))
                {
                    string fileName = Path.Combine(storagePath, "processing.jpg");
                    string newFileName = Path.Combine(storagePath, GetNameFromElapsed(elapsedTime));
                    File.Move(fileName, newFileName);
                }

                Thread.Sleep(interval * 1000);
            }
        }

        private void LoadCamera(string name, int width, int height)
        {
            var devices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));

            DsDevice? device = devices.FirstOrDefault(x => x.Name == name);
            device ??= devices.FirstOrDefault();

            if (device == null)
                throw new Exception("Web Cam not found");

            int index = devices.IndexOf(device);
            _captureDevice = new VideoCapture(index, VideoCapture.API.DShow);
            _captureDevice.Set(CapProp.FrameWidth, width);
            _captureDevice.Set(CapProp.FrameHeight, height);

            if (!_captureDevice.IsOpened)
                throw new Exception("It's not possible to open the Web Cam");

            _frame = new Mat();

            Task.Run(GetFrames);
        }

        private void GetFrames()
        {
            while (!_isDisposing)
            {
                _captureDevice.Read(_frame);
                Thread.Sleep(100);
            }
        }

        private string GetNameFromElapsed(int elapsedTime)
        {
            int hours = elapsedTime / 3600;
            int minutes = (elapsedTime % 3600) / 60;
            int seconds = elapsedTime % 60;
            return $"{hours:D2}-{minutes:D2}-{seconds:D2}.jpg";
        }

        public void Dispose()
        {
            _isDisposing = true;
            _captureDevice.Dispose();
            _frame.Dispose();
        }
    }
}
