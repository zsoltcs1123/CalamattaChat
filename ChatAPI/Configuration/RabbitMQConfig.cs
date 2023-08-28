namespace ChatAPI.Configuration;

public class RabbitMQConfig
{
    public string Hostname { get; set; }
    public string ChatSessionQueueName { get; set; }
    public string ChatFeedbackQueueName { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Hostname))
            throw new InvalidOperationException("Hostname is required.");

        if (string.IsNullOrWhiteSpace(ChatSessionQueueName))
            throw new InvalidOperationException("ChatSession queue name is required.");

        if (string.IsNullOrWhiteSpace(ChatFeedbackQueueName))
            throw new InvalidOperationException("ChatFeedback queue name is required.");
    }
}

