namespace AgentChatCoordinatorService.Configuration;

public class AgentConfig
{
    public int MaximumConcurrentChats { get; set; }

    public bool Validate()
    {
        return MaximumConcurrentChats > 0;
    }
}