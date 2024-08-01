using Comuns.Classes;
using Comuns.Interfaces;
using FileHandler;
using FileHandler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementService
{
    internal class ProcessedSupervisor : Supervisor
    {
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
