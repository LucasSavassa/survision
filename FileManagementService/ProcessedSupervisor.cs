using Comuns.Classes;
using Comuns.Interfaces;
using FileHandler.Services;
using System.IO.Compression;

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

            IResult result = ValidateEntry(entry);

            if (!result.Success)
            {
                foreach (var message in result.Messages) _logger.LogError(message);
                DiscardEntry(entry);
                return;
            }

            CompressAndDelete(entry);
            SendToBlobStorage(entry);
        }

        private void CompressAndDelete(string entry)
        {
            string zipPath = entry + ".zip";
            ZipFile.CreateFromDirectory(entry, zipPath);            
            Directory.Delete(entry, true);
        }

        private void SendToBlobStorage(string entry)
        {
            // TODO: Implement blob storage https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=visual-studio%2Cmanaged-identity%2Croles-azure-portal%2Csign-in-azure-cli%2Cidentity-visual-studio&pivots=blob-storage-quickstart-scratch
            throw new NotImplementedException();
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
