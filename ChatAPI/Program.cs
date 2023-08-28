using System.Text.Json.Serialization;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("/api/chat/poll")))
            .WriteTo.Console()
            .CreateLogger();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Logging
        builder.Services.AddLogging();
        builder.Host.UseSerilog();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }


    private static void ConfigureMiddleware(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
    }
}