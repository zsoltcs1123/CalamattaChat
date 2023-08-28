using System.Text;
using System.Text.Json;
using ChatAPI.Configuration;
using ChatAPI.Services.Chat;
using Messaging.Models;
using Messaging.Services.RabbitMQ;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;

namespace ChatAPI.Services.Messages;

public class ChatSessionFeedbackConsumer : IHostedService, IDisposable
{
    private readonly ILogger<ChatSessionFeedbackConsumer> _logger;
    private readonly IHubContext<ChatSessionHub> _hubContext;
    private readonly RabbitMQConsumer _rabbitMQConsumer;

    public ChatSessionFeedbackConsumer(
        ILogger<ChatSessionFeedbackConsumer> logger,
        IConnection rabbitMQConnection,
        RabbitMQConfig rabbitMQConfig,
        IHubContext<ChatSessionHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
        _rabbitMQConsumer = new RabbitMQConsumer(rabbitMQConnection, rabbitMQConfig.ChatFeedbackQueueName);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMQConsumer.StartConsuming((model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            var chatSessionMessage = JsonSerializer.Deserialize<ChatSessionMessage>(message);

            if (chatSessionMessage == null)
            {
                _logger.LogError("Unable to deserialize message: {Message}", message);
                return;
            }

            _logger.LogInformation("Received feedback for ChatSession {@ChatSession}, new status: {Status}",
                chatSessionMessage.ChatSession,
                chatSessionMessage.ChatSession.Status);
            _logger.LogInformation("Notifying clients through SignalR");

            _hubContext.Clients.All.SendAsync("ReceiveChatSession", chatSessionMessage.ChatSession,
                cancellationToken: cancellationToken);
        });

        return Task.CompletedTask;
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