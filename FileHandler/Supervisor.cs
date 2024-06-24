using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHandler
{
    internal class Supervisor : BackgroundService
    {
        private const int _delay = 5000;
        private const string _queuePath = @"C:\Users\lucas\OneDrive\Desktop\Survision\Queue";

        private readonly ILogger<Supervisor> _logger;

        public Supervisor(ILogger<Supervisor> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("{service} running at: {time}", nameof(Supervisor), DateTimeOffset.Now);
                }

                MonitorQueueFolder();

                await Task.Delay(_delay, stoppingToken);
            }
        }

        private void MonitorQueueFolder()
        {
            if (Directory.Exists(_queuePath))
            {
                string[] files = Directory.GetFiles(_queuePath);
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Processing file: {file}", file);
                        }

                        // Process file
                    }
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("No files to process.");
                    }
                }
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Queue folder does not exist.");
                }
            }

        }
    }
}
