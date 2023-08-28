using AgentChatCoordinatorService.Configuration;
using RabbitMQ.Client;
using Serilog;

const string rabbitMQConfigKey = "RabbitMQConfig";


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Get & validate RabbitMQ config
var rabbitMQConfig = builder.Configuration.GetSection(rabbitMQConfigKey).Get<RabbitMQConfig>();

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
};
var connection = factory.CreateConnection();
builder.Services.AddSingleton(connection);

var app = builder.Build();

app.UseHttpsRedirection();
app.Run();