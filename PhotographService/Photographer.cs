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
using System.Timers;
using Comuns.Interfaces;
using Comuns.Classes;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;

namespace PhotographService
{
    public class Photographer : IDisposable
    {
        private VideoCapture? _captureDevice;
        private Mat? _frame;
        private bool _isDisposing = false;
        private int threadCount = 0;

        public delegate void ShowCapture(Image image);

        public bool IsCapturing { get; private set; }
        public bool Ended { get; private set; }
        public RecordingMetadata? Metadata { get; private set; }

        public Photographer()
        {

        }

        public IResult LoadCamera(string name, int width, int height)
        {
            var devices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));

            DsDevice? device = null;

            if (string.IsNullOrEmpty(name))
            {
                device = devices.FirstOrDefault();
            }
            else
            {
                device = devices.FirstOrDefault(x => x.Name == name);
            }

            if (device == null)
            {
                return Result.Failed("Nenhuma câmera foi encontrada.");
            }

            int index = devices.IndexOf(device);
            _captureDevice = new VideoCapture(index, VideoCapture.API.DShow);
            _captureDevice.Set(CapProp.FrameWidth, width);
            _captureDevice.Set(CapProp.FrameHeight, height);

            if (!_captureDevice.IsOpened)
            {
                return Result.Failed("A câmera não está disponível. Verifique se ela está sendo usada por outro aplicativo.");
            }

            _frame = new Mat();

            return Result.Successfull;
        }

        public void StartGettingFrames()
        {
            if (threadCount == 0) // Only start the thread if it's the first one
            {
                Task.Run(() => GetFrames());
            }
        }

        public bool IsCameraWorking()
        {
            Thread.Sleep(300); // Wait for the first frame to be captured

            if (_captureDevice == null || _frame == null)
                return false;

            if (!_captureDevice.Grab())
                return false;

            Bitmap bitmap = _frame.ToBitmap();

            if (bitmap == null)
                return false;

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            byte[] rgba = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, rgba, 0, rgba.Length);

            bool allBlack = true;
            for (int i = 0; i < rgba.Length; i++)
            {
                if ((i + 1) % 4 == 0) continue; // Skip alpha channel
                if (rgba[i] != 0)
                {
                    allBlack = false;
                    break;
                }
            }

            bitmap.UnlockBits(data);

            if (allBlack)
                return false;

            return true;
        }

        private unsafe void GetFrames()
        {
            threadCount++;

            _isDisposing = false;
            while (!_isDisposing)
            {
                _captureDevice?.Read(_frame);

                Thread.Sleep(100);
            }
            
            threadCount--;
        }

        public void StartCapture(string storagePath, int interval, ShowCapture func)
        {
            if (!IsCapturing)
            {
                IsCapturing = true;
                Ended = false;
                Task.Run(() => CapturePhotos(storagePath, interval, func));
            }
        }

        public async Task StopCaptureAsync()
        {
            IsCapturing = false;
            await WaitEnd();
        }

        private void CapturePhotos(string storagePath, int interval, ShowCapture func)
        {
            DateTime start = DateTime.Now;
            int elapsed = 0;
            int shots = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (IsCapturing)
            {
                elapsed = (int)stopwatch.Elapsed.TotalSeconds;

                if (CapturePhoto(storagePath, func))
                {
                    shots++;
                    string fileName = Path.Combine(storagePath, "processing.jpg");
                    string newFileName = Path.Combine(storagePath, GetNameFromElapsed(elapsed));
                    File.Move(fileName, newFileName);
                }

                Thread.Sleep(interval * 1000);
            }

            this.Metadata = new RecordingMetadata(start, shots, elapsed);
            this.Ended = true;
        }

        public bool CapturePhoto(string storagePath, ShowCapture func)
        {
            if (_frame is null || _frame.IsEmpty)
                return false;

            using (Image image = _frame.ToBitmap())
            {
                string filePath = Path.Combine(storagePath, "processing.jpg");
                image.Save(filePath, ImageFormat.Jpeg);
                func(image);
            }

            return true;
        }

        private string GetNameFromElapsed(int elapsedTime)
        {
            int hours = elapsedTime / 3600;
            int minutes = (elapsedTime % 3600) / 60;
            int seconds = elapsedTime % 60;
            return $"{hours:D2}-{minutes:D2}-{seconds:D2}.jpg";
        }

        public async Task WaitEnd()
        {
            while (!Ended)
            {
                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            _isDisposing = true;

            _captureDevice.Dispose();
            _frame.Dispose();
        }
    }
}
