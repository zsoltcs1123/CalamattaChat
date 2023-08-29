namespace AgentChatCoordinatorService.Configuration;

public class AgentConfig : IConfig
{
    public int MaximumConcurrentChats { get; set; }

    public void Validate()
    {
        if (MaximumConcurrentChats <= 0)
        {
            throw new InvalidOperationException("Maximum concurrent chats must be greater than 0");
        }
    }
}