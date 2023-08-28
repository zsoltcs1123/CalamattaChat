using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Services.Chat;
using AgentChatCoordinatorService.Services.Messaging;
using AgentChatCoordinatorService.Services.Teams;
using Messaging.Services.Chat;
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

//Hosted services
builder.Services.AddHostedService<ChatSessionConsumer>();

// Rest of the services
builder.Services.AddSingleton<ITeamService, TeamService>();
builder.Services.AddSingleton<IChatAssignmentService, ChatAssignmentService>();
builder.Services.AddSingleton<IChatSessionPublisher, ChatSessionPublisher>();
builder.Services.AddSingleton<IAgentPublisher, AgentPublisher>();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

app.UseHttpsRedirection();
app.Run();