namespace AgentChatCoordinatorService.Services.Messaging;

public interface IAgentPublisher
{
    void Publish(AgentAssignedMessage agentAssignedMessage);
}