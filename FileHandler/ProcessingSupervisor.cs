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
                if (!FoldersExists())
                {
                    _logger.LogError("There are missing folders.");
                    CreateFolders();
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

        private bool FoldersExists()
        {
            return Directory.Exists(_processingPath)
                && Directory.Exists(_binPath)
                && Directory.Exists(_processedPath);
        }

        private void CreateFolders()
        {
            _logger.LogInformation("Creating folders.");
            Directory.CreateDirectory(_processingPath);
            Directory.CreateDirectory(_binPath);
            Directory.CreateDirectory(_processedPath);
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

            ProcessFilesAtFolder(entry);

            Directory.Delete(entry, false);
        }

        private void ProcessFilesAtFolder(string folder)
        {
            string destination = Path.Combine(_processedPath, folder);
            Directory.CreateDirectory(destination);
            using FileStream resultStream = File.Create(Path.Combine(destination, "results.json"));

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
                    Match match = Regex.Match(fileName, _imageFileNamePattern);

                    string hoursText = match.Groups["hours"].Value;
                    string minutesText = match.Groups["minutes"].Value;
                    string secondsText = match.Groups["seconds"].Value;

                    uint hours = uint.Parse(hoursText);
                    uint minutes = uint.Parse(minutesText);
                    uint seconds = uint.Parse(secondsText);

                    uint second = hours * 3600 + minutes * 60 + seconds;

                    using (Bitmap bitmap = new(file))
                    {
                        IPredictionResult result = _imagePredictionService.GetImageResults(bitmap, _threshold, 0.1, true).Result;
                        PictureResult pictureResult = new PictureResult() { Second = second, Detections = result.Predictions };
                        pictureResult.Second = second;
                        surgeryResult.Timeline.Add(pictureResult);
                    }

                    File.Move(file, Path.Combine(destination, fileName));
                    continue;
                }
                else
                {
                    DiscardEntry(file, folderName);
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
    }
}