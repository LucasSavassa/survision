using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileHandler
{
    internal class Supervisor : BackgroundService
    {
        private const int _delay = 5000;
        private const string _queuePath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Queue";
        private const string _processingPath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Processing";
        private const string _binPath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Bin";
        private const string _fileNamePattern = @"\bsurgery-[0-9]{1,11}\b";

        private readonly ILogger<Supervisor> _logger;

        public Supervisor(ILogger<Supervisor> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("{service} running at: {time}", nameof(Supervisor), DateTimeOffset.Now);
                }

                MonitorQueueFolder();

                await Task.Delay(_delay, stoppingToken);
            }
        }

        private void MonitorQueueFolder()
        {
            if (Directory.Exists(_queuePath))
            {
                string[] files = Directory.GetFiles(_queuePath);
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Processing file: {file}", file);
                        }

                        ProcessFile(file);
                    }
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("No files to process.");
                    }
                }
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Queue folder does not exist.");
                }
            }

        }

        private void ProcessFile(string file)
        {
            string extension = Path.GetExtension(file);
            string fileName = Path.GetFileName(file);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

            if (extension != ".zip")
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Invalid file extension: {extension}", extension);
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving file to bin folder: {file}", file);
                }

                File.Move(file, Path.Combine(_binPath, fileName));

                return;
            }

            if (!Regex.IsMatch(fileNameWithoutExtension, _fileNamePattern))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Invalid file name: {fileName}", fileName);
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving file to bin folder: {file}", file);
                }

                File.Move(file, Path.Combine(_binPath, fileName));

                return;
            }

            if (!ContentIsValid(file, out ICollection<string> messages))
            {
                foreach (var message in messages)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(message);
                    }
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving file to bin folder: {file}", file);
                }

                File.Move(file, Path.Combine(_binPath, fileName));

                return;
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Moving file to processing folder: {file}", file);
            }

            string destination = Path.Combine(_processingPath, Path.GetFileName(file));
            File.Move(file, destination);
        }

        private bool ContentIsValid(string file, out ICollection<string> messages)
        {
            messages = new List<string>();

            bool hasMetadata = false;
            bool hasImage = false;

            using (ZipArchive archive = ZipFile.OpenRead(file))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName == "metadata.json")
                    {
                        hasMetadata = true;
                        continue;
                    }

                    if (entry.FullName.EndsWith(".jpg") || entry.FullName.EndsWith(".jpeg") || entry.FullName.EndsWith(".png"))
                    {
                        hasImage = true;
                        continue;
                    }
                }
            }

            if (!hasMetadata)
            {
                messages.Add("metadata.json file not found.");
            }

            if (!hasImage)
            {
                messages.Add("Image file not found.");
            }

            return hasMetadata && hasImage;
        }
    }
}
