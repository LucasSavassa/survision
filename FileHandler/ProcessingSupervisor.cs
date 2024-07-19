using Comuns.Classes;
using Comuns.Interfaces;
using CustomVisionPredictionService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileHandler
{
    internal class ProcessingSupervisor : Supervisor
    {
        private const string _folderNamePattern = @"\bsurgery-(?<id>[0-9]{1,11})\b";
        private const string _imageFileNamePattern = @"\b(?<hours>[0-9]{2})-(?<minutes>[0-9]{2})-(?<seconds>[0-9]{2})\b";

        private readonly IPredictionService _imagePredictionService;
        private int _threshold = 50;

        public ProcessingSupervisor(ILogger<ProcessingSupervisor> logger) : base(logger)
        {
            _imagePredictionService = new ImagePredictionCustomVision();
        }

        protected override void Monitor()
        {
            try
            {
                if (!Directory.Exists(_processingPath))
                {
                    _logger.LogError("Processing folder does not exist.");
                    Directory.CreateDirectory(_processingPath);
                    return;
                }
                string[] entries = Directory.GetFileSystemEntries(_processingPath);
                if (entries.Length == 0)
                {
                    _logger.LogError("No entry to process.");
                    return;
                }

                foreach (var entry in entries)
                {
                    ProcessEntry(entry);
                }

                ResetExceptions();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing the processing folder.");

                HandleException();
            }
        }
        private void ProcessEntry(string entry)
        {
            _logger.LogInformation("Processing entry: {entry}", entry);

            string folderName = Path.GetFileName(entry);

            FileAttributes attributes = File.GetAttributes(entry);

            if (!attributes.HasFlag(FileAttributes.Directory))
            {
                _logger.LogError("Invalid entry type: {entry}", entry);
                DiscardEntry(entry);
                return;
            }

            if (!Regex.IsMatch(folderName, _folderNamePattern))
            {
                _logger.LogError("Invalid folder name: {folderName}", folderName);
                DiscardEntry(entry);
                return;
            }

            string destination = Path.Combine(_processedPath, folderName);
            Directory.CreateDirectory(destination);
            using FileStream resultStream = File.Create(Path.Combine(destination, "results.json"));
            ProcessFilesAtFolder(entry, destination, resultStream);

            Directory.Delete(entry, false);
        }

        private void ProcessFilesAtFolder(string folder, string destination, FileStream resultStream)
        {
            string folderName = Path.GetFileName(folder);
            SurgeryResult surgeryResult = new();

            foreach (var file in Directory.GetFiles(folder))
            {
                string fileName = Path.GetFileName(file);
                string fileExtension = Path.GetExtension(file);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                string bin = Path.Combine(_binPath, folderName);

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
                        _logger.LogError("{filename} is an invalid name for an image file.", fileName);
                        DiscardEntry(file, folderName);
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
                            _logger.LogError("The image filename does not contain a valid hour indicator.");
                            DiscardEntry(file, folderName);
                            continue;
                        }

                        if (hours < 0)
                        {
                            _logger.LogError("The image filename does not contain a valid hour indicator.");
                            DiscardEntry(file, folderName);
                            continue;
                        }

                        if (!uint.TryParse(minutesText, out minutes))
                        {
                            _logger.LogError("The image filename does not contain a valid minute indicator.");
                            DiscardEntry(file, folderName);
                            continue;
                        }

                        if (minutes < 0 || minutes > 60)
                        {
                            _logger.LogError("The image filename does not contain a valid minute indicator.");
                            DiscardEntry(file, folderName);
                            continue;
                        }

                        if (!uint.TryParse(secondsText, out seconds))
                        {
                            _logger.LogError("The image filename does not contain a valid second indicator.");
                            DiscardEntry(file, folderName);
                            continue;
                        }

                        if (seconds < 0 || seconds > 60)
                        {
                            _logger.LogError("The image filename does not contain a valid second indicator.");
                            DiscardEntry(file, folderName);
                            continue;
                        }
                    }

                    uint second = hours * 3600 + minutes * 60 + seconds;

                    using (Bitmap bitmap = new(file))
                    {
                        IPredictionResult result = _imagePredictionService.GetImageResults(bitmap, _threshold, 0.1, false).Result;
                        PictureResult pictureResult = SerializePredictionResult(second, result);
                        pictureResult.Second = second;
                        surgeryResult.Timeline.Add(pictureResult);
                    }

                    File.Move(file, Path.Combine(destination, fileName));
                    continue;
                }
                else
                {
                    if (!Directory.Exists(bin))
                    {
                        Directory.CreateDirectory(bin);
                    }
                    File.Move(file, Path.Combine(bin, fileName));
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

        private PictureResult SerializePredictionResult(uint second, IPredictionResult result)
        {
            return new()
            {
                Second = second,
                Detections = result.Predictions
            };
        }
    }
}
