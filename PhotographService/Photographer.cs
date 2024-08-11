using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using AForge.Video.DirectShow;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace PhotographService
{
    public class Photographer : IDisposable
    {
        private VideoCapture _captureDevice;
        private bool _isCapturing;


        public Photographer(string cameraName, int desiredWidth, int desiredHeight)
        {
            LoadCamera(cameraName, desiredWidth, desiredHeight);
        }

        public bool CapturePhoto(string storagePath)
        {
            using (var frame = new Mat())
            {
                _captureDevice.Read(frame);

                if (frame.IsEmpty)
                    return false;

                using (Image image = frame.ToBitmap())
                {
                    string filePath = Path.Combine(storagePath, "processing.jpg");
                    image.Save(filePath);
                }
                
                return true;
            }
        }

        public void StartCapture(string storagePath, int interval,int durationSeconds)
        {
            _isCapturing = true;
            Task.Run(() => CapturePhotos(storagePath, interval, durationSeconds));
        }

        public void StopCapture()
        {
            _isCapturing = false;
        }

        private void CapturePhotos(string storagePath, int interval, int durationSeconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            int elapsedTime = 0;
            stopwatch.Start();

            while(_isCapturing)
            {
                if (CapturePhoto(storagePath))
                {
                    elapsedTime = (int)stopwatch.Elapsed.TotalSeconds;
                    string fileName = Path.Combine(storagePath, "processing.jpg");
                    string newFileName = Path.Combine(storagePath, GetNameFromElapsed(elapsedTime));
                    File.Move(fileName, newFileName);
                }

                if (elapsedTime > durationSeconds)
                    break;

                Thread.Sleep(interval * 1000);
            }
        }

        private void LoadCamera(string cameraName, int desiredWidth, int desiredHeight)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            for (int i = 0; i < videoDevices.Count; i++)
            {
                if (videoDevices[i].Name == cameraName)
                {
                    _captureDevice = new VideoCapture(i);
                    _captureDevice.Set(CapProp.FrameWidth, desiredWidth);
                    _captureDevice.Set(CapProp.FrameHeight, desiredHeight);
                    break;
                }
            }

            if (_captureDevice == null)
                throw new Exception("Web Cam not found");

            if (!_captureDevice.IsOpened)
                throw new Exception("It's not possible to open the Web Cam");
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
            _captureDevice.Dispose();
        }
    }
}
