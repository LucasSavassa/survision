using Comuns.Classes;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            app.MapGet("/surgery/{surgeryName}", (string surgeryName) =>
            {
                SurgeryResult? surgery = Handler.GetSurgeryJson(surgeryName);

                if (surgery == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(surgery);
            });

            app.MapGet("/surgery/{room}/{year}/{month}/{day}/{hour}/{minute}/{second}", (int room, int year, int month, int day, int hour, int minute, int second) =>
            {
                SurgeryResult? surgery = Handler.GetSurgeryJson(room, year, month, day, hour, minute, second);

                if (surgery == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(surgery);
            });

            app.MapGet("/surgery/{room}/{year}/{month}/{day}", (int room, int year, int month, int day) =>
            {
                IEnumerable<SurgeryResult?> surgeries = Handler.GetSurgeryJson(room, year, month, day);

                if (surgeries == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(surgeries);
            });

            app.MapGet("/surgery/usage/{surgeryName}", (string surgeryName) =>
            {
                IDictionary<string, int>? usage = Handler.GetSurgeryUsage(surgeryName);

                if (usage == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(usage);
            });

            app.MapGet("/surgery/usage/{room}/{year}/{month}/{day}/{hour}/{minute}/{second}", (int room, int year, int month, int day, int hour, int minute, int second) =>
            {
                IDictionary<string, int>? usages = Handler.GetSurgeryUsage(room, year, month, day, hour, minute, second);

                if (usages == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(usages);
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }
            app.Run();
        }
    }
}
