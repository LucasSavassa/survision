using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace PhotographService
{
    public class PhotographService
    {
        private readonly MediaCapture _mediaCapture;
        private readonly ApplicationDataContainer _localSettings;
        private readonly ILogger _logger;
        private int _captureInterval = 10;
        private bool _isCapturing = false;

        public PhotographService(ILogger<PhotographService> logger) : this()
        {
            _logger = logger;
        }

        public PhotographService()
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

        public async Task StartCaptureAsync(CancellationToken token)
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
                    await CapturePhotoAsync();
                    await Task.Delay(TimeSpan.FromSeconds(_captureInterval), token);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Stopped capturing.");
                return;
            }
        }

        public void StopCapture()
        {
            _isCapturing = false;
        }

        private async Task CapturePhotoAsync()
        {
            StorageLibrary pictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            StorageFolder root = await pictures.SaveFolder.CreateFolderAsync("Survision", CreationCollisionOption.OpenIfExists);
            StorageFile photoFile = await root.CreateFileAsync("photo.jpg", CreationCollisionOption.GenerateUniqueName);

            using (var captureStream = new InMemoryRandomAccessStream())
            {
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                using (var photoStream = await photoFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var decoder = await BitmapDecoder.CreateAsync(captureStream);
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(photoStream, decoder);
                    var imageProperties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) } };
                    await encoder.BitmapProperties.SetPropertiesAsync(imageProperties);
                    await encoder.FlushAsync();
                }
            }

            ImageProperties fileProperties = await photoFile.Properties.GetImagePropertiesAsync();
            DateTimeOffset dateTaken = fileProperties.DateTaken;
            await photoFile.RenameAsync($"{dateTaken:HH-mm-ss}.jpg", NameCollisionOption.GenerateUniqueName);
        }
    }
}
