namespace AgentChatCoordinatorService.Configuration;

public class RabbitMQConfig : IConfig
{
    public string Hostname { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string ChatSessionQueueName { get; set; }
    public string ChatFeedbackQueueName { get; set; }
    public string AgentQueueName { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Username))
            throw new InvalidOperationException("Username is required.");

        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException("Password is required.");
        
        if (string.IsNullOrWhiteSpace(Hostname))
            throw new InvalidOperationException("Hostname is required.");

        if (string.IsNullOrWhiteSpace(ChatSessionQueueName))
            throw new InvalidOperationException("ChatSession queue name is required.");

        if (string.IsNullOrWhiteSpace(ChatFeedbackQueueName))
            throw new InvalidOperationException("ChatFeedback queue name is required.");
        
        if (string.IsNullOrWhiteSpace(ChatFeedbackQueueName))
            throw new InvalidOperationException("Agent queue name is required.");
    }
}