using Comuns.Classes;
using Comuns.Interfaces;
using FileHandler.Services;
using FileManagementService.Services;
using System.IO.Compression;
using System.Text.Json;

namespace FileManagementService
{
    internal class ProcessedSupervisor : Supervisor
    {
        protected override string MainPath => ProcessedPath;

        public ProcessedSupervisor(ILogger<ProcessedSupervisor> logger) : base(logger)
        {

        }

        protected override bool FoldersExist()
        {
            return Directory.Exists(ProcessedPath)
                && Directory.Exists(BinPath);
        }

        protected override void CreateFolders()
        {
            _logger.LogInformation("Creating folders.");
            Directory.CreateDirectory(ProcessedPath);
            Directory.CreateDirectory(BinPath);
        }

        protected override void ProcessEntry(string entry)
        {
            _logger.LogInformation("Processing entry: {entry}", entry);

            if (ShouldSkip(entry))
            {
                return;
            }

            IResult result = ValidateEntry(entry);

            if (!result.Success)
            {
                foreach (var message in result.Messages) _logger.LogError(message);
                DiscardEntry(entry);
                return;
            }

            Trim(entry);
            DateTime surgeryDay = Validator.GetSurgeryDateFromFolderName(entry);
            int room = Validator.GetSurgeryRoomFromFolderName(entry);
            Librarian.MoveToGallery(entry, room, surgeryDay, GalleryPath);
        }

        private bool ShouldSkip(string entry)
        {
            string entryName = Path.GetFileNameWithoutExtension(entry);
            string extension = Path.GetExtension(entry);

            bool isTemp = (extension == "" && entryName.StartsWith("temp-"));
            bool isTrimmed = (extension == ".zip" && entryName.StartsWith("trim-"));

            return isTemp || isTrimmed;
        }

        private void Trim(string entry)
        {
            string resultsPath = Path.Combine(entry, "results.json");
            string content = File.ReadAllText(resultsPath);
            SurgeryResult? surgeryResult = JsonSerializer.Deserialize<SurgeryResult>(content);

            if (surgeryResult is null) return;

            int lastHash = 0;

            for (int i = surgeryResult.Timeline.Count() - 1; i >= 0; i--)
            {
                PictureResult element = surgeryResult.Timeline.ElementAt(i);
                if (element.Hash == lastHash)
                {
                    surgeryResult.Timeline.Remove(element);
                }
                else
                {
                    lastHash = element.Hash;
                }
            }

            string newContent = JsonSerializer.Serialize(surgeryResult);
            File.WriteAllText(resultsPath, newContent);
        }

        protected override IResult ValidateEntry(string entry)
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
    }
}
