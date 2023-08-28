using System.Text.Json;
using AgentChatCoordinatorService.Configuration;
using Messaging.Services.RabbitMQ;
using RabbitMQ.Client;

namespace AgentChatCoordinatorService.Services.Messaging;

public class AgentPublisher : IAgentPublisher
{
    private readonly ILogger<AgentPublisher> _logger;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private readonly IConnection _rabbitMQConnection;

    public AgentPublisher(   
        ILogger<AgentPublisher> logger,
        RabbitMQConfig rabbitMQConfig,
        IConnection rabbitMQConnection)
    {
        _logger = logger;
        _rabbitMQConfig = rabbitMQConfig;
        _rabbitMQConnection = rabbitMQConnection;
    }

    public void Publish(AgentAssignedMessage agentAssignedMessage)
    {
        using var publisher = new RabbitMQPublisher(_rabbitMQConnection, _rabbitMQConfig.AgentQueueName);
        string jsonString = JsonSerializer.Serialize(agentAssignedMessage);
        
        publisher.PublishMessage(jsonString);
        
        _logger.LogInformation("AgentAssignedMessage {@AgentAssignedMessage} sent to RabbitMQ queue '{QueueName}'", 
            agentAssignedMessage,
            _rabbitMQConfig.AgentQueueName);
    }
}