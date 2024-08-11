using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace PhotographService
{
    public class Photographer
    {
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private readonly ILogger _logger;
        private int _captureInterval = 10;
        private bool _isCapturing = false;
        private MediaCaptureInitializationSettings _mediaSettings;

        public int CaptureInterval
        {
            get
            {
                return _captureInterval;
            }
            set
            {
                _captureInterval = value;
                _localSettings.Values["CaptureInterval"] = _captureInterval;
            }
        }

        public bool IsCapturing => _isCapturing;

        public Photographer(ILogger<Photographer> logger) : this()
        {
            _logger = logger;
        }

        public Photographer()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (_localSettings.Values.ContainsKey("CaptureInterval"))
            {
                _captureInterval = (int)_localSettings.Values["CaptureInterval"];
            }
            else
            {
                _localSettings.Values["CaptureInterval"] = _captureInterval;
            }
        }

        private async Task LoadCamera()
        {
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (devices.Count == 0) throw new InvalidOperationException("No camera found.");

            IEnumerable<DeviceInformation> match = devices.Where(d => d.Name == "HD Pro Webcam C920");
            DeviceInformation device = match.FirstOrDefault() ?? devices.First();
            _mediaSettings = new MediaCaptureInitializationSettings { VideoDeviceId = device.Id };
        }

        public async Task StartCaptureAsync(string storagePath, CancellationToken token)
        {
            try
            {
                await LoadCamera();
                token.ThrowIfCancellationRequested();

                if (_isCapturing)
                {
                    _logger.LogInformation("Already capturing.");
                    return;
                }

                _isCapturing = true;
                Stopwatch stopwatch = Stopwatch.StartNew();

                while (_isCapturing)
                {
                    await CapturePhotoAsync(storagePath, stopwatch);
                    await Task.Delay(TimeSpan.FromSeconds(_captureInterval), token);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Cancellation requested.");
                return;
            }
        }

        public void StopCapture()
        {
            _isCapturing = false;
        }

        public async Task<StorageFile> CapturePhotoAsync(string storagePath, Stopwatch stopwatch)
        {
            int elapsed = 0;
            string name = string.Empty;
            StorageFolder storage = await StorageFolder.GetFolderFromPathAsync(storagePath);
            StorageFile photoFile = await storage.CreateFileAsync("processing.jpg", CreationCollisionOption.GenerateUniqueName);

            using (MediaCapture mediaCapture = new MediaCapture())
            {
                await mediaCapture.InitializeAsync(_mediaSettings);
                var allVideoProperties = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview).Cast<VideoEncodingProperties>().ToList();
                var desiredResolution = allVideoProperties.FirstOrDefault(r => r.Width == 640 && r.Height == 360);

                await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, desiredResolution);

                using (var captureStream = new InMemoryRandomAccessStream())
                {
                    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                    elapsed = (int)stopwatch.Elapsed.TotalSeconds;
                    using (var photoStream = await photoFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await CreatePhotoAsync(captureStream, photoStream);
                    }
                }
            }
            name = GetNameFromElapsed(elapsed);
            await photoFile.RenameAsync(name, NameCollisionOption.FailIfExists);
            return photoFile;
        }

        private string GetNameFromElapsed(int elapsed)
        {
            int hours = elapsed / 3600;
            int minutes = (elapsed % 3600) / 60;
            int seconds = elapsed % 60;
            return $"{hours:D2}-{minutes:D2}-{seconds:D2}.jpg";
        }

        private static async Task CreatePhotoAsync(InMemoryRandomAccessStream captureStream, IRandomAccessStream photoStream)
        {
            var decoder = await BitmapDecoder.CreateAsync(captureStream);
            var encoder = await BitmapEncoder.CreateForTranscodingAsync(photoStream, decoder);
            var imageProperties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) } };
            await encoder.BitmapProperties.SetPropertiesAsync(imageProperties);
            await encoder.FlushAsync();
        }
    }
}
