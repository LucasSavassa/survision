using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using CustomVisionPredictionService;
using CustomVisionPredictionService.ViewObjects;
using Comuns.Classes;
using Comuns.Interfaces;

namespace FileHandler
{
    internal class Supervisor : BackgroundService
    {
        private int _threshold = 50;
        private int _delay = 5000;
        private int _consecutiveErrors = 0;
        private int _exceptionPolicy = 0;
        private const string _queuePath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Queue";
        private const string _processingPath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Processing";
        private const string _processedPath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Processed";
        private const string _binPath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Bin";
        private const string _folderNamePattern = @"\bsurgery-(?<id>[0-9]{1,11})\b";
        private const string _imageFileNamePattern = @"\b(?<hours>[0-9]{2})-(?<minutes>[0-9]{2})-(?<seconds>[0-9]{2})";

        private readonly ImagePrediction _imagePredictor = new ImagePrediction();
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
            try
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
            catch (Exception exception)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(exception, "An error occurred while processing the queue folder.");
                }

                HandleException();
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
            try
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

                ResetExceptions();
            }
            catch (Exception exception)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(exception, "An error occurred while processing the processing folder.");
                }

                HandleException();
            }
        }

        private void ProcessEntryAtProcessing(string entry)
        {
            string folderName = Path.GetFileName(entry);
            int surgeryId = -1;

            if (_logger.IsEnabled(LogLevel.Information))
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

            Match match = Regex.Match(folderName, _folderNamePattern);
            if (match.Success) 
            {
                string? id = match.Groups["id"]?.Value;
                if (string.IsNullOrWhiteSpace(id))
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("Invalid folder name: {folderName}", folderName);
                    }
                    return;
                }
                if (!int.TryParse(id, out surgeryId))
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("Invalid id at folder name: {folderName}", folderName);
                    }
                    return;
                }
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
                    _logger.LogInformation("Moving entry to bin folder: {entry}.", entry);
                }

                Directory.Move(entry, Path.Combine(_binPath, folderName));
                return;
            }

            if (!IsMetadataValid(metadataPath, surgeryId))
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("metadata.json file is not valid");
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
                    _logger.LogInformation("Moving entry to bin folder: {entry}.", entry);
                }

                Directory.Move(entry, Path.Combine(_binPath, folderName));
                return;
            }

            string destination = Path.Combine(_processedPath, folderName);
            Directory.CreateDirectory(destination);
            using (FileStream resultStream = File.Create(Path.Combine(destination, "results.json")))
            {
                ProcessFilesAtFolder(entry, destination, resultStream);
            }

            Directory.Delete(entry, false);
        }

        private bool IsMetadataValid(string metadataPath, int surgeryId)
        {
            try
            {
                string content = File.ReadAllText(metadataPath);
         
                Surgery? surgery = JsonSerializer.Deserialize<Surgery>(content);

                if (surgery is null)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("Error deserializing metadata.json.");
                    }
                    return false;
                }

                if (surgery.Id <= 0 || surgery.Id != surgeryId)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("The metadata.json contains a surgery id that is different from the folder id.");
                    }
                    return false;
                }

                if (surgery.Type == Comuns.Enums.SurgeryType.None)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("The metadata.json contains an invalid surgery type.");
                    }
                    return false;
                }

                if (surgery.Shots <= 0)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("The metadata.json contains an invalid quantity of shots.");
                    }
                    return false;
                }

                if (surgery.Seconds <= 0)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("The metadata.json contains an invalid quantity of seconds.");
                    }
                    return false;
                }

                if (surgery.Start == default)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("The metadata.json contains an invalid start date.");
                    }
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(exception, "An error occurred while deserializing metadata.json file.");
                }

                return false;
            }
        }

        private void ProcessFilesAtFolder(string entry, string destination, FileStream resultStream)
        {
            string folderName = Path.GetFileName(entry);
            SurgeryResult surgeryResult = new();

            foreach (var file in Directory.GetFiles(entry))
            {
                string fileName = Path.GetFileName(file);
                string fileExtension = Path.GetExtension(file);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                if (fileName == "metadata.json")
                {
                    surgeryResult.Surgery = GetSurgeryDataFromMetadataFile(file);
                    File.Move(file, Path.Combine(destination, fileName));
                    continue;
                }
                else if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                {
                    uint hours = 0;
                    uint minutes = 0;
                    uint seconds = 0;

                    if (!Regex.IsMatch(fileName, _imageFileNamePattern))
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError("{filename} is an invalid name for an image file.", fileName);
                        }

                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Moving {filename} to the bin folder.", fileName);
                        }

                        File.Move(file, Path.Combine(_binPath, folderName, fileName));
                        continue;
                    }

                    Match match = Regex.Match(fileName, _imageFileNamePattern);

                    if (match.Success)
                    {
                        string hoursText = match.Groups["hours"].Value;
                        string minutesText = match.Groups["minutes"].Value;
                        string secondsText = match.Groups["seconds"].Value;

                        if (!uint.TryParse(hoursText, out hours))
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("The image filename does not contain a valid hour indicator.");
                            }

                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation("Moving {filename} to the bin folder.", fileName);
                            }

                            File.Move(file, Path.Combine(_binPath, folderName, fileName));

                            continue;
                        }

                        if (hours < 0)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("The image filename does not contain a valid hour indicator.");
                            }

                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation("Moving {filename} to the bin folder.", fileName);
                            }

                            File.Move(file, Path.Combine(_binPath, folderName, fileName));

                            continue;
                        }

                        if (!uint.TryParse(minutesText, out minutes))
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("The image filename does not contain a valid minute indicator.");
                            }

                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation("Moving {filename} to the bin folder.", fileName);
                            }

                            File.Move(file, Path.Combine(_binPath, folderName, fileName));

                            continue;
                        }

                        if (minutes < 0 || minutes > 60)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("The image filename does not contain a valid minute indicator.");
                            }

                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation("Moving {filename} to the bin folder.", fileName);
                            }

                            File.Move(file, Path.Combine(_binPath, folderName, fileName));

                            continue;
                        }

                        if (!uint.TryParse(secondsText, out seconds))
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("The image filename does not contain a valid second indicator.");
                            }

                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation("Moving {filename} to the bin folder.", fileName);
                            }

                            File.Move(file, Path.Combine(_binPath, folderName, fileName));

                            continue;
                        }

                        if (seconds < 0 || seconds > 60)
                        {
                            if (_logger.IsEnabled(LogLevel.Error))
                            {
                                _logger.LogError("The image filename does not contain a valid second indicator.");
                            }

                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation("Moving {filename} to the bin folder.", fileName);
                            }

                            File.Move(file, Path.Combine(_binPath, folderName, fileName));

                            continue;
                        }
                    }

                    uint durationSeconds = hours * 3600 + minutes * 60 + seconds;

                    using (FileStream stream = new(file, FileMode.Open, FileAccess.Read))
                    {
                        PredictionCustomVisionVO result = _imagePredictor.GetImageResults(stream, _threshold, true).Result;
                        PictureResult pictureResult = SerializePredictionResult(result);
                        pictureResult.Second = durationSeconds;
                        surgeryResult.Timeline.Add(pictureResult);
                    }
                    File.Move(file, Path.Combine(destination, fileName));
                    continue;
                }
                else
                {
                    File.Move(file, Path.Combine(_binPath, folderName, fileName));
                }
            }

            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(surgeryResult);
            resultStream.Write(bytes, 0, bytes.Length);
        }

        private Surgery GetSurgeryDataFromMetadataFile(string file)
        {
            string content = File.ReadAllText(file);
            return JsonSerializer.Deserialize<Surgery>(content) ?? new();
        }

        private PictureResult SerializePredictionResult(PredictionCustomVisionVO result)
        {
            return new()
            {
                Second = 0,
                Detections = result.PredictionDatas.ToList()
            };
        }

        private void ResetExceptions()
        {
            _consecutiveErrors = 0;
            _exceptionPolicy = 0;

            ImplementNewExceptionPolicy();
        }

        private void HandleException()
        {
            _consecutiveErrors++;

            if (_consecutiveErrors >= 3 && _consecutiveErrors < 12)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"The number of consecutive exceptions reached {_consecutiveErrors}, raising exception policy to 1.");
                }
                _exceptionPolicy = 1;
            }
            else if (_consecutiveErrors >= 12 && _consecutiveErrors < 48)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"The number of consecutive exceptions reached {_consecutiveErrors}, raising exception policy to 2.");
                }
                _exceptionPolicy = 2;
            }
            else if (_consecutiveErrors >= 48)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"The number of consecutive exceptions reached {_consecutiveErrors}, stopping service.");
                }
                _exceptionPolicy = 3;
            }

            ImplementNewExceptionPolicy();
        }

        private void ImplementNewExceptionPolicy()
        {
            switch (_exceptionPolicy)
            {
                case 1: _delay = 600000; break;
                case 2: _delay = 3600000; break;
                case 3: Environment.Exit(1); break;
                default: _delay = 5000; break;
            }
        }
    }
}