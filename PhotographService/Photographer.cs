using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public bool IsCapturing => _isCapturing;

        public Photographer(ILogger<Photographer> logger) : this()
        {
            _logger = logger;
        }

        public Photographer()
        {
            LoadSettings();
            LoadCamera();
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

        private void LoadCamera()
        {
            DeviceInformationCollection devices = DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).AsTask().Result;

            if (devices.Count == 0) throw new InvalidOperationException("No camera found.");

            IEnumerable<DeviceInformation> match = devices.Where(d => d.Name == "HD Pro Webcam C920");
            DeviceInformation device = match.FirstOrDefault() ?? devices.First();
            _mediaSettings = new MediaCaptureInitializationSettings { VideoDeviceId = device.Id };
        }

        public async Task StartCaptureAsync(StorageFolder storage, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (_isCapturing)
                {
                    _logger.LogInformation("Already capturing.");
                    return;
                }

                _isCapturing = true;

                while (_isCapturing)
                {
                    await CapturePhotoAsync(storage);
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

        public async Task<StorageFile> CapturePhotoAsync(StorageFolder storage)
        {
            StorageFile photoFile = await CreatePhotoFileAsync(storage);

            using (MediaCapture mediaCapture = new MediaCapture())
            {
                await mediaCapture.InitializeAsync(_mediaSettings);
                using (var captureStream = new InMemoryRandomAccessStream())
                {
                    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                    using (var photoStream = await photoFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await CreatePhotoAsync(captureStream, photoStream);
                    }
                }
            }

            await RenamePhoto(photoFile);
            return photoFile;
        }

        private async Task<StorageFile> CreatePhotoFileAsync(StorageFolder storage)
        {
            StorageFile photoFile = await storage.CreateFileAsync("processing.jpg", CreationCollisionOption.GenerateUniqueName);

            return photoFile;
        }

        private static async Task CreatePhotoAsync(InMemoryRandomAccessStream captureStream, IRandomAccessStream photoStream)
        {
            var decoder = await BitmapDecoder.CreateAsync(captureStream);
            var encoder = await BitmapEncoder.CreateForTranscodingAsync(photoStream, decoder);
            var imageProperties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) } };
            await encoder.BitmapProperties.SetPropertiesAsync(imageProperties);
            await encoder.FlushAsync();
        }

        private static async Task RenamePhoto(StorageFile photoFile)
        {
            ImageProperties fileProperties = await photoFile.Properties.GetImagePropertiesAsync();
            DateTimeOffset dateTaken = fileProperties.DateTaken;
            await photoFile.RenameAsync($"{dateTaken:HH-mm-ss}.jpg", NameCollisionOption.GenerateUniqueName);
        }
    }
}
