using Comuns.Classes;
using Comuns.Enums;
using Comuns.Interfaces;
using Comuns.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileHandler
{
    internal class QueueSupervisor : Supervisor
    {
        private const string _folderNamePattern = @"\bsurgery-(?<id>[0-9]{1,11})\b";
        private const string _imageFileNamePattern = @"\b(?<hours>[0-9]{2})-(?<minutes>[0-9]{2})-(?<seconds>[0-9]{2})\b";

        public QueueSupervisor(ILogger<QueueSupervisor> logger) : base(logger)
        {

        }

        protected override void Monitor()
        {
            try
            {
                if (!FoldersExists())
                {
                    _logger.LogError("Queue folder does not exist.");
                    CreateFolders();
                    return;
                }

                string[] entries = Directory.GetFileSystemEntries(_queuePath);

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
                _logger.LogError(exception, "An error occurred while processing the queue folder.");
                HandleException();
            }
        }

        private bool FoldersExists()
        {
            return Directory.Exists(_queuePath)
                && Directory.Exists(_binPath)
                && Directory.Exists(_processingPath);
        }

        private void CreateFolders()
        {
            _logger.LogInformation("Creating folders.");
            Directory.CreateDirectory(_queuePath);
            Directory.CreateDirectory(_binPath);
            Directory.CreateDirectory(_processingPath);
        }

        private void ProcessEntry(string entry)
        {
            _logger.LogInformation("Processing entry: {entry}", entry);

            IResult result = ValidateQueueEntry(entry);

            if (!result.Success)
            {
                foreach (var message in result.Messages) _logger.LogError(message);
                DiscardEntry(entry);
                return;
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(entry);
            string destination = Path.Combine(_processingPath, fileNameWithoutExtension);

            if (File.Exists(destination))
            {
                _logger.LogError("Entry already exists in processing folder.");
                DiscardEntry(entry);
                return;
            }

            ZipFile.ExtractToDirectory(entry, destination);
            File.Delete(entry);
        }

        public static IResult ValidateQueueEntry(string entry)
        {
            string extension = Path.GetExtension(entry);
            string entryName = Path.GetFileName(entry);
            string entryNameWithoutExtension = Path.GetFileNameWithoutExtension(entry);

            if (extension != ".zip")
            {
                return new Result()
                {
                    Success = false,
                    Messages = ["Invalid file extension."]
                };
            }

            if (!Regex.IsMatch(entryNameWithoutExtension, _folderNamePattern))
            {
                return new Result()
                {
                    Success = false,
                    Messages = ["Invalid entry name."]
                };
            }

            string idText = Regex.Match(entryNameWithoutExtension, _folderNamePattern).Groups["id"].Value;

            if (!int.TryParse(idText, out int id))
            {
                return new Result()
                {
                    Success = false,
                    Messages = ["Invalid entry name."]
                };
            }

            if (!HasOneMetadataAndImage(entry, out ICollection<string> messages))
            {
                return new Result()
                {
                    Success = false,
                    Messages = messages
                };
            }

            return Result.Successfull;
        }

        private static bool HasOneMetadataAndImage(string zip, out ICollection<string> messages)
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
                        if (IsValidMetadata(entry, zip, messages))
                        {
                            metadataCount++;
                        }
                        continue;
                    }
                    else if (entry.FullName.EndsWith(".jpg") || entry.FullName.EndsWith(".jpeg") || entry.FullName.EndsWith(".png"))
                    {
                        if (IsValidImage(entry))
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
            else if (metadataCount >= 2)
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

        private static bool IsValidMetadata(ZipArchiveEntry metadata, string zip, ICollection<string> messages)
        {
            string zipName = Path.GetFileNameWithoutExtension(zip);
            string idText = Regex.Match(zipName, _folderNamePattern).Groups["id"].Value;

            if (!int.TryParse(idText, out int surgeryId))
            {
                messages.Add($"Invalid zip name: {zipName}");
                return false;
            }

            using Stream file = metadata.Open();
            using StreamReader reader = new(file);
            string json = reader.ReadToEnd();

            Surgery? surgery;

            try
            {
                surgery = JsonSerializer.Deserialize<Surgery>(json);
            }
            catch (JsonException exception)
            {
                messages.Add($"Exception deserializing metadata.json: {exception.Message}");
                return false;
            }

            if (surgery is null)
            {
                messages.Add("Error deserializing metadata.json.");
                return false;
            }

            if (surgery.Id <= 0 || surgery.Id != surgeryId)
            {
                messages.Add("The metadata.json contains a surgery id that is different from the folder id.");
                return false;
            }

            if (surgery.Type == Comuns.Enums.SurgeryType.None)
            {
                messages.Add("The metadata.json contains an invalid surgery type.");
                return false;
            }

            if (!surgery.Type.IsDefined())
            {
                messages.Add("The metadata.json contains an invalid surgery type."); 
                return false;
            }

            if (surgery.Shots <= 0)
            {
                messages.Add("The metadata.json contains an invalid quantity of shots.");
                return false;
            }

            if (surgery.Seconds <= 0)
            {
                messages.Add("The metadata.json contains an invalid quantity of seconds.");
                return false;
            }

            if (surgery.Start == default)
            {
                messages.Add("The metadata.json contains an invalid start date.");
                return false;
            }

            return true;
        }

        private static bool IsValidImage(ZipArchiveEntry entry)
        {
            return Regex.IsMatch(entry.FullName, _imageFileNamePattern);
        }
    }
}