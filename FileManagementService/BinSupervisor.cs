using Comuns.Classes;
using Comuns.Interfaces;
using FileHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementService
{
    internal class BinSupervisor : Supervisor
    {
        private const int _secondsToLive = 604800;
        private DateTime _oldestFile = DateTime.Now;

        protected override string MainPath => BinPath;

        public BinSupervisor(ILogger<BinSupervisor> logger) : base(logger)
        {

        }

        protected override void CreateFolders()
        {
            Directory.CreateDirectory(BinPath);
        }

        protected override bool FoldersExist()
        {
            return Directory.Exists(BinPath);
        }

        protected override void Monitor()
        {
            try
            {
                if (!FoldersExist())
                {
                    _logger.LogError("There are missing folders.");
                    CreateFolders();
                    return;
                }

                string[] entries = Directory.GetFileSystemEntries(MainPath);

                if (entries.Length == 0)
                {
                    _logger.LogError("No entry to process.");
                    Delay = 5000;
                    return;
                }

                foreach (var entry in entries)
                {
                    ProcessEntry(entry);
                }

                AdjustDelay();

                ResetExceptionPolicy();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing the processing folder.");

                HandleException();
            }
        }

        protected override void ProcessEntry(string entry)
        {
            DateTime createdAt = File.GetCreationTime(entry);
            _oldestFile = createdAt < _oldestFile ? createdAt : _oldestFile;
            
            DateTime threshold = DateTime.Now.AddSeconds(-_secondsToLive);
            if (createdAt < threshold)
            {
                _logger.LogInformation($"Deleting file at bin {entry}.");
                File.Delete(entry);
            }
        }

        private void AdjustDelay()
        {
            DateTime threshold = DateTime.Now.AddSeconds(-_secondsToLive);
            Delay = (((_oldestFile - threshold).Seconds) + 3600) * 1000;
        }

        protected override IResult ValidateEntry(string entry)
        {
            return Result.Successfull;
        }
    }
}
