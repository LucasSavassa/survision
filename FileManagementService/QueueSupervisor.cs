using Comuns.Classes;
using Comuns.Interfaces;
using FileHandler.Services;
using System.IO.Compression;

namespace FileManagementService
{
    internal class QueueSupervisor : Supervisor
    {
        protected override string MainPath => QueuePath;

        public QueueSupervisor(ILogger<QueueSupervisor> logger) : base(logger)
        {

        }

        protected override bool FoldersExist()
        {
            return Directory.Exists(QueuePath)
                && Directory.Exists(BinPath)
                && Directory.Exists(ProcessingPath);
        }

        protected override void CreateFolders()
        {
            _logger.LogInformation("Creating folders.");
            Directory.CreateDirectory(QueuePath);
            Directory.CreateDirectory(BinPath);
            Directory.CreateDirectory(ProcessingPath);
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

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(entry);
            string destination = Path.Combine(ProcessingPath, fileNameWithoutExtension);

            if (File.Exists(destination))
            {
                _logger.LogError("Entry already exists in processing folder.");
                DiscardEntry(entry);
                return;
            }

            ZipFile.ExtractToDirectory(entry, destination);
            File.Delete(entry);
        }

        override protected IResult ValidateEntry(string entry)
        {
            string extension = Path.GetExtension(entry);
            string entryName = Path.GetFileName(entry);
            string entryNameWithoutExtension = Path.GetFileNameWithoutExtension(entry);

            if (extension != ".zip")
            {
                return new Result(false, null, ["Invalid file extension."]);
            }

            if (!Validator.IsValidSurgeryFolderName(entryNameWithoutExtension))
            {
                return new Result(false, null, ["Invalid entry name."]);
            }

            if (!HasValidContent(entry, out ICollection<string> messages))
            {
                return new Result(false, null, messages);
            }

            return Result.Successfull;
        }

        private static bool HasValidContent(string zip, out ICollection<string> messages)
        {
            messages = [];

            int metadataCount = 0;
            bool hasOneAndOnlyOneMetadata = false;
            bool hasImage = false;

            using (ZipArchive archive = ZipFile.OpenRead(zip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName == "metadata.json")
                    {
                        IResult result = Validator.IsValidMetadataFile(entry, zip);
                        if (result.Success)
                        {
                            metadataCount++;
                        }
                        else
                        {
                            foreach (var message in result.Messages) messages.Add(message);
                        }
                        continue;
                    }
                    else if (entry.FullName.EndsWith(".jpg") || entry.FullName.EndsWith(".jpeg") || entry.FullName.EndsWith(".png"))
                    {
                        if (Validator.IsValidImageFileName(entry.Name))
                        {
                            hasImage = true;
                        }
                        continue;
                    }
                    else
                    {
                        messages.Add($"Invalid file found, deleting it: {entry.FullName}");
                        entry.Delete();
                    }
                }
            }

            if (metadataCount <= 0)
            {
                messages.Add("metadata.json file not found.");
            }
            else if (metadataCount > 1)
            {
                messages.Add("More than one metadata.json file found.");
            }
            else
            {
                hasOneAndOnlyOneMetadata = true;
            }

            if (!hasImage)
            {
                messages.Add("Image file not found.");
            }

            return hasOneAndOnlyOneMetadata && hasImage;
        }
    }
}