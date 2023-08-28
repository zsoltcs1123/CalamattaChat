using ChatAPI.Configuration;
using Messaging.Models;
using Messaging.Services.Chat;
using SharedModels.Entities;
using SharedModels.Enums;

namespace ChatAPI.Services.Monitor;

public class PollRequestMonitoringService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IChatSessionPublisher _chatSessionPublisher;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private readonly IPollRequestMonitor _sessionMonitor;

    public PollRequestMonitoringService(
        IChatSessionPublisher chatSessionPublisher,
        RabbitMQConfig rabbitMQConfig,
        IPollRequestMonitor sessionMonitor)
    {
        _chatSessionPublisher = chatSessionPublisher;
        _rabbitMQConfig = rabbitMQConfig;
        _sessionMonitor = sessionMonitor;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var inactiveSessionIds = _sessionMonitor.CheckMissedPolls().ToList();
        if (!inactiveSessionIds.Any()) return;
        
        foreach (var chatSession in inactiveSessionIds)
        {
            chatSession.Status = ChatSessionStatus.Inactive;
            PublishInactiveChatSessionToQueue(chatSession);
        }
    }

    private void PublishInactiveChatSessionToQueue(ChatSession chatSession)
    {
        _chatSessionPublisher.Publish(new ChatSessionMessage(chatSession), _rabbitMQConfig.ChatSessionQueueName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
