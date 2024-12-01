using FileManagementService;

namespace FileHandler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddHostedService<QueueSupervisor>();
            builder.Services.AddHostedService<ProcessingSupervisor>();
            builder.Services.AddHostedService<ProcessedSupervisor>();
            builder.Services.AddHostedService<BinSupervisor>();

            var host = builder.Build();
            host.Run();
        }
    }
}