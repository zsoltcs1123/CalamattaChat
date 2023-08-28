using System.Text.Json;
using Messaging.Models;
using Messaging.Services.RabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Messaging.Services.Chat;

public class ChatSessionPublisher : IChatSessionPublisher
{
    private readonly ILogger<ChatSessionPublisher> _logger;
    private readonly IConnection _rabbitMQConnection;

    public ChatSessionPublisher(   
        ILogger<ChatSessionPublisher> logger,
        IConnection rabbitMQConnection)
    {
        _logger = logger;
        _rabbitMQConnection = rabbitMQConnection;
    }

    public void Publish(ChatSessionMessage chatSessionMessage, string queueName)
    {
        using var publisher = new RabbitMQPublisher(_rabbitMQConnection, queueName);
        string jsonString = JsonSerializer.Serialize(chatSessionMessage);
        
        publisher.PublishMessage(jsonString);
        
        _logger.LogInformation("ChatSessionMessage {@ChatSessionMessage} sent to RabbitMQ queue '{QueueName}'", 
            chatSessionMessage,
            queueName);
    }
    
}