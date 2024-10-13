using Comuns.Classes;
using Comuns.Enums;
using Comuns.Interfaces;
using System.IO.Compression;
using System.Text.Json;

namespace FileManagementService
{
    public abstract class Supervisor : BackgroundService
    {
        protected readonly ILogger<Supervisor> _logger;

        private int _delay = 5000;
        private int _consecutiveErrors = 0;
        private int _exceptionPolicy = 0;

        public static string RootPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Survision");
        public static string QueuePath => Path.Combine(RootPath, "Queue");
        public static string ProcessingPath => Path.Combine(RootPath, "Processing");
        public static string ProcessedPath => Path.Combine(RootPath, "Processed");
        public static string GalleryPath => Path.Combine(RootPath, "Gallery");
        public static string BinPath => Path.Combine(RootPath, "Bin");
        public static NeuralNetworkType NeuralNetwork { get; set; } = NeuralNetworkType.Yolo;
        public static int Threshold { get; set; } = 50;

        protected int Delay 
        {
            get { return _delay; }
            set { _delay = value; }
        }

        abstract protected string MainPath { get; }

        protected Supervisor(ILogger<Supervisor> logger)
        {
            _logger = logger;
        }

        abstract protected bool FoldersExist();
        abstract protected void CreateFolders();
        abstract protected void ProcessEntry(string entry);
        abstract protected IResult ValidateEntry(string entry);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("{service} running at: {time}", nameof(Supervisor), DateTimeOffset.Now);

                Monitor();

                await Task.Delay(_delay, stoppingToken);
            }
        }

        protected virtual void Monitor()
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
                    return;
                }

                foreach (var entry in entries)
                {
                    ProcessEntry(entry);
                }

                ResetExceptionPolicy();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing the processing folder.");

                HandleException();
            }
        }

        public static string CreateTemporaryFolder(int room, DateTime start)
        {
            string path = CreateFolder(room, start);
            CreateMetadata(room, start, path);
            return path;
        }

        private static string CreateFolder(int room, DateTime start)
        {
            string path = Path.Combine(RootPath, $"surgery-{room}-{start:yyyyMMddHHmmss}");
            Directory.CreateDirectory(path);
            return path;
        }

        private static void CreateMetadata(int room, DateTime start, string path)
        {
            Surgery surgery = new Surgery
            {
                Room = room,
                Start = start
            };

            string json = JsonSerializer.Serialize(surgery);
            string filePath = Path.Combine(path, "metadata.json");
            File.WriteAllText(filePath, json);
        }

        public static void UpdateMetadata(string path, int shots, int seconds)
        {
            string json = File.ReadAllText(path);
            Surgery? surgery = JsonSerializer.Deserialize<Surgery>(json);
            if (surgery is null) return;
            surgery.Shots = (uint)shots;
            surgery.Seconds = (uint)seconds;
            json = JsonSerializer.Serialize(surgery);
            File.WriteAllText(path, json);
        }

        public static string ZipFolder(string path)
        {
            string name = Path.GetFileName(path);
            string directory = Path.GetDirectoryName(path) ?? ""; 
            string destination = Path.Combine(directory, $"{name}.zip");
            ZipFile.CreateFromDirectory(path, destination);
            Directory.Delete(path, true);
            return destination;
        }

        public static void MoveToQueue(string path)
        {
            string name = Path.GetFileName(path);
            string destination = Path.Combine(QueuePath, name);
            Directory.Move(path, destination);
        }

        protected void MoveToGallery(string entry, int room, DateTime date)
        {
            string zipped = ZipFolder(entry);
            string name = Path.GetFileName(zipped);
            string destinationFolder = Path.Combine(GalleryPath, $"{room}", $"{date.Year}", $"{date.Month}", $"{date.Day}");
            Directory.CreateDirectory(destinationFolder);
            string destination = Path.Combine(destinationFolder, name);
            Directory.Move(zipped, destination);
        }

        public static IEnumerable<string> GetSurgeriesAtGallery(int room, DateTime date)
        {
            string path = Path.Combine(
                GalleryPath,
                $"{room}",
                $"{date.Year}",
                $"{date.Month}",
                $"{date.Day}"
            );

            if (!Directory.Exists(path))
            {
                return Enumerable.Empty<string>();
            }

            return Directory.EnumerateFiles(path);
        }

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

        protected void ResetExceptionPolicy()
        {
            _consecutiveErrors = 0;
            _exceptionPolicy = 0;
            _delay = 5000;
        }

        protected void HandleException()
        {
            IncrementExceptionPolicy();
            ImplementExceptionPolicy(_exceptionPolicy);
        }

        private void IncrementExceptionPolicy()
        {
            _consecutiveErrors++;

            switch (_consecutiveErrors)
            {
                case >= 3 and < 12:
                    _logger.LogInformation($"The number of consecutive exceptions reached {_consecutiveErrors}, the exception policy is now 1.");
                    _exceptionPolicy = 1;
                    break;
                case >= 12 and < 48:
                    _logger.LogInformation($"The number of consecutive exceptions reached {_consecutiveErrors}, the exception policy is now 2.");
                    _exceptionPolicy = 2;
                    break;
                case >= 48:
                    _logger.LogInformation($"The number of consecutive exceptions reached {_consecutiveErrors}, stopping service.");
                    _exceptionPolicy = 3;
                    break;
            }
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