namespace FileHandler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<QueueSupervisor>();

            var host = builder.Build();
            host.Run();
        }
    }
}