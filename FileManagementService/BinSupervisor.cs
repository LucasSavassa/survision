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
        private const int _timeToLive = 604800;

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

        protected override void ProcessEntry(string entry)
        {
            throw new NotImplementedException();
            // TODO: Scan through files and delete those that are older than _timeToLive
            // TODO: Plan delay based on the oldest file
        }

        protected override IResult ValidateEntry(string entry)
        {
            return Result.Successfull;
        }
    }
}
