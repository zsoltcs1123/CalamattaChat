using System.Text.Json.Serialization;
using ChatAPI.Configuration;
using ChatAPI.Services.Chat;
using ChatAPI.Services.Messages;
using ChatAPI.Services.Monitor;
using Messaging.Services.Chat;
using RabbitMQ.Client;
using Serilog;

namespace ChatAPI;

public class Program
{
    private const string RabbitMQConfigKey = "RabbitMQConfig";
    private const string ChatSessionHubRoute = "api/chatHub";

    public static void Main(string[] args)
    {
        ConfigureLogging();

        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);
        ConfigureRabbitMQ(builder);
        ConfigureSignalR(builder);

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
    
    private static void ConfigureRabbitMQ(WebApplicationBuilder builder)
    {
        // Get & validate RabbitMQ config
        var rabbitMQConfig = builder.Configuration.GetSection(RabbitMQConfigKey).Get<RabbitMQConfig>();

        if (rabbitMQConfig == null)
        {
            throw new InvalidOperationException("Unable to construct RabbitMQ config model");
        }

        rabbitMQConfig.Validate();
        builder.Services.AddSingleton(rabbitMQConfig);

        // Create & register RabbitMQ connection object
        var factory = new ConnectionFactory
        {
            HostName = rabbitMQConfig.Hostname,
            Port = rabbitMQConfig.Port,
            UserName = rabbitMQConfig.Username,
            Password = rabbitMQConfig.Password
        };
        var connection = factory.CreateConnection();
        builder.Services.AddSingleton(connection);
    }
    
    private static void ConfigureSignalR(WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
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
        
        // Hosted services
        builder.Services.AddHostedService<PollRequestMonitoringService>();
        builder.Services.AddHostedService<ChatSessionFeedbackConsumer>();

        // Rest of the services
        builder.Services.AddSingleton<IPollRequestMonitor, PollRequestMonitor>();
        builder.Services.AddSingleton<IChatSessionPublisher, ChatSessionPublisher>();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ChatSessionHub>(ChatSessionHubRoute);
        });
    }
}