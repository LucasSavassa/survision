using Comuns.Classes;
using Comuns.Enums;
using Comuns.Interfaces;
using Comuns.Extension;
using FileHandler.Services;
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
        public QueueSupervisor(ILogger<QueueSupervisor> logger) : base(logger)
        {

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

            IResult result = ValidateEntry(entry);

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