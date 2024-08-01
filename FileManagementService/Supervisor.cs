using Comuns.Classes;
using Comuns.Enums;
using Comuns.Interfaces;
using CustomVisionPredictionService;
using System.Drawing;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using YoloPredictionService;

namespace FileHandler
{
    public abstract class Supervisor : BackgroundService
    {
        protected readonly ILogger<Supervisor> _logger;

        private int _delay = 5000;
        private int _consecutiveErrors = 0;
        private int _exceptionPolicy = 0;

        private readonly string _rootPath;
        private readonly string _queuePath;
        private readonly string _processingPath;
        private readonly string _processedPath;
        private readonly string _binPath;

        protected string RootPath => _rootPath;
        protected string QueuePath => _queuePath;
        protected string ProcessingPath => _processingPath;
        protected string ProcessedPath => _processedPath;
        protected string BinPath => _binPath;

        protected Supervisor(ILogger<Supervisor> logger)
        {
            _logger = logger;
            _rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Survision");
            _queuePath = Path.Combine(_rootPath, "Queue");
            _processingPath = Path.Combine(_rootPath, "Processing");
            _processedPath = Path.Combine(_rootPath, "Processed");
            _binPath = Path.Combine(_rootPath, "Bin");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("{service} running at: {time}", nameof(Supervisor), DateTimeOffset.Now);

                Monitor();

                await Task.Delay(_delay, stoppingToken);
            }
        }

        abstract protected void Monitor();
        abstract protected IResult ValidateEntry(string entry);

        protected void DiscardEntry(string entry, string destFolderName = "")
        {
            string name = Path.GetFileName(entry);
            _logger.LogInformation("Moving {name} to bin", name);

            Guid guid = Guid.NewGuid();
            string newBin = Path.Combine(BinPath, guid.ToString(), destFolderName);
            Directory.CreateDirectory(newBin);

            string destination = Path.Combine(newBin, name);
            Directory.Move(entry, destination);
        }

        protected void ResetExceptions()
        {
            _consecutiveErrors = 0;
            _exceptionPolicy = 0;
            _delay = 5000;
        }

        protected void HandleException()
        {
            _consecutiveErrors++;

            if (_consecutiveErrors >= 3 && _consecutiveErrors < 12)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"The number of consecutive exceptions reached {_consecutiveErrors}, the exception policy is now 1.");
                }
                _exceptionPolicy = 1;
            }
            else if (_consecutiveErrors >= 12 && _consecutiveErrors < 48)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"The number of consecutive exceptions reached {_consecutiveErrors}, the exception policy is now 2.");
                }
                _exceptionPolicy = 2;
            }
            else if (_consecutiveErrors >= 48)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError($"The number of consecutive exceptions reached {_consecutiveErrors}, stopping service.");
                }
                _exceptionPolicy = 3;
            }

            ImplementExceptionPolicy(_exceptionPolicy);
        }

        protected void ImplementExceptionPolicy(int exceptionPolicy)
        {
            switch (exceptionPolicy)
            {
                case 1: _delay = 600000; break;
                case 2: _delay = 3600000; break;
                case 3: Environment.Exit(1); break;
                default: _delay = 5000; break;
            }
        }
    }
}