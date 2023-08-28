using Messaging.Models;

namespace Messaging.Services.Chat;

public interface IChatSessionPublisher
{
    void Publish(ChatSessionMessage chatSessionMessage, string queueName);
}