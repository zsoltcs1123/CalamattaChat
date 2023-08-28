using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Services.Agents;

public interface IAgentGenerator
{
    IEnumerable<Agent> GenerateAgents(string teamName, int count, AgentSeniority seniority);
}