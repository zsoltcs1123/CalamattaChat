using System.Text.Json;
using AgentChatCoordinatorService.Configuration;
using Messaging.Models;
using Messaging.Services.RabbitMQ;
using RabbitMQ.Client;
using SharedModels.Entities;
using SharedModels.Enums;

namespace AgentChatCoordinatorService.Services.Messaging;

public class ChatSessionConsumer : IHostedService, IDisposable
{
    private readonly ILogger<ChatSessionConsumer> _logger;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private readonly RabbitMQConsumer _rabbitMQConsumer;

    public ChatSessionConsumer(
        ILogger<ChatSessionConsumer> logger,
        IConnection rabbitMQConnection,
        RabbitMQConfig rabbitMQConfig)
    {
        _logger = logger;
        _rabbitMQConfig = rabbitMQConfig;

        _rabbitMQConsumer = new RabbitMQConsumer(rabbitMQConnection, _rabbitMQConfig.ChatSessionQueueName);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMQConsumer.StartConsuming((model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = System.Text.Encoding.UTF8.GetString(body);
            ChatSessionMessage? chatSessionMessage = JsonSerializer.Deserialize<ChatSessionMessage>(message);

            if (chatSessionMessage == null)
            {
                _logger.LogError("Unable to deserialize message: {Message}", message);
                return;
            }

            _logger.LogInformation("ChatSessionMessage {@ChatSessionMessage} received", chatSessionMessage);

            if (chatSessionMessage.ChatSession.Status == ChatSessionStatus.Initiated)
            {
                AssignChatSessionToAgent(chatSessionMessage.ChatSession);
                return;
            }

            if (chatSessionMessage.ChatSession.Status == ChatSessionStatus.Inactive)
            {
                //Handle inactive chat - not specified in task
                _logger.LogInformation("ChatSession {@ChatSession} marked as inactive", chatSessionMessage.ChatSession);
            }
        });

        return Task.CompletedTask;
    }

    private void AssignChatSessionToAgent(ChatSession chatSession)
    {
        return;
    }
    

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _rabbitMQConsumer.Dispose();
    }
}