using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Services.Agents;
using AgentChatCoordinatorService.Services.Chat;
using AgentChatCoordinatorService.Services.Messaging;
using AgentChatCoordinatorService.Services.Teams;
using Messaging.Services.Chat;
using RabbitMQ.Client;
using Serilog;

namespace AgentChatCoordinatorService // Replace with appropriate namespace
{
    public class Program
    {
        private const string RabbitMQConfigKey = "RabbitMQConfig";
        private const string TeamsConfigKey = "Teams";
        private const string AgentConfigKey = "Agent";
        private const string OfficeHoursConfigKey = "OfficeHours";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // Get and validate configuration
            ConfigureApplication(builder);
            
            // Setup RabbitMQ and services
            ConfigureServices(builder);

            var app = builder.Build();
            app.UseHttpsRedirection();
            app.Run();
        }

        private static void ConfigureApplication(WebApplicationBuilder builder)
        {
            // Get config objects
            var rabbitMQConfig = builder.Configuration.GetSection(RabbitMQConfigKey).Get<RabbitMQConfig>();
            var agentConfig = builder.Configuration.GetSection(AgentConfigKey).Get<AgentConfig>();
            var teamConfigs = builder.Configuration.GetSection(TeamsConfigKey).Get<List<TeamConfig>>();
            if (teamConfigs == null)
            {
                throw new InvalidOperationException("Unable to construct Teams config model");
            }
            var teamsConfig = new TeamsConfig(teamConfigs);

            foreach (var teamConfig in teamConfigs)
            {
                teamConfig.Validate();
            }

            var officeHoursConfig = builder.Configuration.GetSection(OfficeHoursConfigKey).Get<OfficeHoursConfig>();
            var configs = new IConfig?[] { rabbitMQConfig, agentConfig, teamsConfig, officeHoursConfig };

            ValidateConfigs(configs, builder);
            
            builder.Services.AddSingleton(rabbitMQConfig);
            builder.Services.AddSingleton(agentConfig);
            builder.Services.AddSingleton(teamsConfig);
            builder.Services.AddSingleton(officeHoursConfig);
        }

        private static void ValidateConfigs(IEnumerable<IConfig?> configs, WebApplicationBuilder builder)
        {
            foreach (var config in configs)
            {
                if (config == null)
                {
                    throw new InvalidOperationException($"Unable to construct config model");
                }
                config.Validate();
            }
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Create & register RabbitMQ connection object
            var rabbitMQConfig = builder.Services.BuildServiceProvider().GetRequiredService<RabbitMQConfig>();
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQConfig.Hostname,
            };
            var connection = factory.CreateConnection();
            builder.Services.AddSingleton(connection);

            // Hosted services
            builder.Services.AddHostedService<ChatSessionConsumer>();

            // Rest of the services
            builder.Services.AddSingleton<ITeamService, TeamService>();
            builder.Services.AddSingleton<ITeamGenerator, TeamGenerator>();
            builder.Services.AddSingleton<IAgentGenerator, AgentGenerator>();
            builder.Services.AddSingleton<IChatAssignmentService, ChatAssignmentService>();
            builder.Services.AddSingleton<IChatSessionPublisher, ChatSessionPublisher>();
            builder.Services.AddSingleton<IAgentPublisher, AgentPublisher>();

            // Logging
            builder.Services.AddLogging();
        }
    }
}
