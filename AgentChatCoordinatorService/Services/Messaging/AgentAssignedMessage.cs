using AgentChatCoordinatorService.Models;
using SharedModels.Entities;

namespace AgentChatCoordinatorService.Services.Messaging;

public class AgentAssignedMessage
{
    public ChatSession ChatSession { get; set; }
    public Agent Agent { get; set; }

    public AgentAssignedMessage(ChatSession chatSession, Agent agent)
    {
        ChatSession = chatSession;
        Agent = agent;
    }
}