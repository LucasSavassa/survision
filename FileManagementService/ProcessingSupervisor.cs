using Comuns.Classes;
using Comuns.Enums;
using Comuns.Interfaces;
using CustomVisionPredictionService;
using FileHandler.Services;
using System.Drawing;
using System.Drawing.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using YoloPredictionService;

namespace FileManagementService
{
    internal class ProcessingSupervisor : Supervisor
    {
        protected override string MainPath => ProcessingPath;

        public ProcessingSupervisor(ILogger<ProcessingSupervisor> logger) : base(logger) { }

        protected override bool FoldersExist()
        {
            return Directory.Exists(ProcessingPath)
                && Directory.Exists(BinPath)
                && Directory.Exists(ProcessedPath);
        }

        protected override void CreateFolders()
        {
            _logger.LogInformation("Creating folders.");
            Directory.CreateDirectory(ProcessingPath);
            Directory.CreateDirectory(BinPath);
            Directory.CreateDirectory(ProcessedPath);
        }

        protected override void ProcessEntry(string entry)
        {
            _logger.LogInformation("Processing entry: {entry}", entry);

            IResult result = ValidateEntry(entry);

            if (!result.Success)
            {
                foreach (var message in result.Messages) _logger.LogError(message);
                DiscardEntry(entry);
                return;
            }

            ProcessFilesAtFolder(entry);

            Directory.Delete(entry, false);
        }

        override protected IResult ValidateEntry(string entry)
        {
            ICollection<string> messages = [];

            string entryName = Path.GetFileNameWithoutExtension(entry);
            string extension = Path.GetExtension(entry);

            if (extension != string.Empty)
            {
                messages.Add("Invalid entry type.");
                return new Result(false, null, messages);
            }

            if (!Validator.IsValidSurgeryFolderName(entry))
            {
                messages.Add("Invalid folder name.");
                return new Result(false, null, messages);
            }

            return Result.Successfull;
        }

        private void ProcessFilesAtFolder(string folder)
        {
            string folderName = Path.GetFileName(folder);
            string tempDestination = Path.Combine(ProcessedPath, $"temp-{folderName}");
            Directory.CreateDirectory(tempDestination);
            using (FileStream resultStream = File.Create(Path.Combine(tempDestination, "results.json")))
            {
                SurgeryResult surgeryResult = new();

                foreach (var file in Directory.GetFiles(folder))
                {
                    string fileName = Path.GetFileName(file);
                    string fileExtension = Path.GetExtension(file);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string bin = Path.Combine(BinPath, folderName);

                    if (fileName == "metadata.json")
                    {
                        IResult result = Validator.IsValidMetadataFile(file, folder);
                        if (!result.Success)
                        {
                            DiscardEntry(file, folderName);
                            continue;
                        }
                        surgeryResult.Surgery = GetSurgeryDataFromMetadataFile(file);
                        File.Move(file, Path.Combine(tempDestination, fileName));
                    }
                    else if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        IResult result = Validator.IsValidImageFile(file);

                        if (!result.Success)
                        {
                            DiscardEntry(file, folderName);
                            continue;
                        }

                        Match match = Validator.MatchImageFileNamePattern(fileNameWithoutExtension);

                        string hoursText = match.Groups["hours"].Value;
                        string minutesText = match.Groups["minutes"].Value;
                        string secondsText = match.Groups["seconds"].Value;

                        uint hours = uint.Parse(hoursText);
                        uint minutes = uint.Parse(minutesText);
                        uint seconds = uint.Parse(secondsText);

                        uint second = hours * 3600 + minutes * 60 + seconds;

                        using (Bitmap bitmap = new(file))
                        {
                            IPredictionService predictionService = GetPredictionService();
                            IPredictionResult predictionResult = predictionService.GetImageResults(bitmap, Threshold);
                            PictureResult pictureResult = new() { Second = second, Detections = predictionResult.Predictions };
                            surgeryResult.Timeline.Add(pictureResult);
                        }

                        File.Move(file, Path.Combine(tempDestination, fileName));
                    }
                    else
                    {
                        DiscardEntry(file, folderName);
                    }
                }

                byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(surgeryResult);
                resultStream.Write(bytes, 0, bytes.Length);
            }
            string destination = Path.Combine(ProcessedPath, folderName);
            RenameTempFolder(tempDestination, destination);
        }

        private static void RenameTempFolder(string tempDestination, string destination)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Directory.Move(tempDestination, destination);

                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(50);
                    if (i == 9) throw;
                }
            }
        }

        private IPredictionService GetPredictionService()
        {
            switch (NeuralNetwork)
            {
                case NeuralNetworkType.CustomVision:
                    return new ImagePredictionCustomVision();
                case NeuralNetworkType.Yolo:
                default:
                    return new ImagePredictionYolo();
            }
        }

        private Surgery GetSurgeryDataFromMetadataFile(string file)
        {
            string content = File.ReadAllText(file);
            return JsonSerializer.Deserialize<Surgery>(content) ?? new();
        }
    }
}