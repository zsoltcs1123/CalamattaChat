using System.Text.Json;
using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Models;
using AgentChatCoordinatorService.Services.Chat;
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
    private readonly IChatAssignmentService _chatAssignmentService;

    public ChatSessionConsumer(
        ILogger<ChatSessionConsumer> logger,
        IConnection rabbitMQConnection,
        RabbitMQConfig rabbitMQConfig,
        IChatAssignmentService chatAssignmentService)
    {
        _logger = logger;
        _rabbitMQConfig = rabbitMQConfig;
        _chatAssignmentService = chatAssignmentService;

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
        if (!IsAssigned(chatSession, out var agent))
        {
            chatSession.Status = ChatSessionStatus.Refused;

            _logger.LogInformation("ChatSession {@ChatSession} refused", chatSession);
        }
        else
        {
            chatSession.Status = ChatSessionStatus.Assigned;
            //Todo send feedback message
            //Todo publish on agent queue
        }
    }

    private bool IsAssigned(ChatSession chatSession, out Agent? agent)
    {
        return _chatAssignmentService.TryAssignChatToAgent(chatSession, out agent);
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