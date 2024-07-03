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
        private const string _processedPath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Processed";
        private const string _binPath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Bin";
        private const string _folderNamePattern = @"\bsurgery-[0-9]{1,11}\b";

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
                MonitorProcessingFolder();

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

                        ProcessFileAtQueue(file);
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

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Creating queue folder: {queuePath}", _queuePath);
                }

                Directory.CreateDirectory(_queuePath);
            }
        }

        private void ProcessFileAtQueue(string file)
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

            if (!Regex.IsMatch(fileNameWithoutExtension, _folderNamePattern))
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

            if (!QueueFileIsValid(file, out ICollection<string> messages))
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

            string destination = Path.Combine(_processingPath, fileNameWithoutExtension);

            if (File.Exists(destination))
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving file to bin folder: {file}", file);
                }

                File.Move(file, _binPath);
            }
            else
            {
                ZipFile.ExtractToDirectory(file, destination);
                File.Delete(file);
            }
        }

        private bool QueueFileIsValid(string file, out ICollection<string> messages)
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

        private void MonitorProcessingFolder()
        {
            if (Directory.Exists(_processingPath))
            {
                string[] entries = Directory.GetFileSystemEntries(_processingPath);
                if (entries.Length > 0)
                {
                    foreach (var entry in entries)
                    {
                        ProcessEntryAtProcessing(entry);
                    }
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("No entries to process.");
                    }
                }
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Processing folder does not exist.");
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Creating processing folder: {processingPath}", _processingPath);
                }

                Directory.CreateDirectory(_processingPath);
            }
        }

        private void ProcessEntryAtProcessing(string entry)
        {
            string folderName = Path.GetFileName(entry);

            if(_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Processing entry: {entry}", entry);
            }

            FileAttributes attributes = File.GetAttributes(entry);

            if (!attributes.HasFlag(FileAttributes.Directory))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Invalid entry type: {entry}", entry);
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving entry to bin folder: {entry}", entry);
                }

                File.Move(entry, Path.Combine(_binPath, folderName));
                return;
            }
            
            if (!Regex.IsMatch(folderName, _folderNamePattern))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Invalid folder name: {folderName}", folderName);
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving entry to bin folder: {entry}", entry);
                }

                Directory.Move(entry, Path.Combine(_binPath, folderName));
                return;
            }

            string metadataPath = Path.Combine(entry, "metadata.json");

            if (!File.Exists(metadataPath))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("metadata.json file not found.");
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving entry to bin folder: {entry}", entry);
                }

                Directory.Move(entry, Path.Combine(_binPath, folderName));
                return;
            }

            if (Directory.GetFiles(entry, "*.jpg").Length == 0 && Directory.GetFiles(entry, "*.jpeg").Length == 0 && Directory.GetFiles(entry, "*.png").Length == 0)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Image file not found.");
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Moving entry to bin folder: {entry}", entry);
                }

                Directory.Move(entry, Path.Combine(_binPath, folderName));
                return;
            }

            string destination = Path.Combine(_processedPath, folderName);
            Directory.CreateDirectory(destination);
            File.Create(Path.Combine(destination, "results.json")).Close();

            ProcessFilesAtFolder(entry, destination);

            Directory.Delete(entry, false);
        }

        private void ProcessFilesAtFolder(string entry, string destination)
        {
            string folderName = Path.GetFileName(entry);

            foreach(var file in Directory.GetFiles(entry))
            {
                string fileName = Path.GetFileName(file);
                string fileExtension = Path.GetExtension(file);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                if (fileName == "metadata.json")
                {
                    File.Move(file, Path.Combine(destination, fileName));
                    continue;
                }
                else if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                {
                    // Send image to AI service
                    // Save results to results.json file
                    File.Move(file, Path.Combine(destination, fileName));
                    continue;
                }
                else
                {
                    string bin = Path.Combine(_binPath, folderName);
                    if(!Directory.Exists(bin))
                    {
                        Directory.CreateDirectory(bin);
                    }
                    File.Move(file, Path.Combine(bin, fileName));
                }
            }
        }
    }
}
