using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Extensions;
using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Services.Agents;

public class AgentGenerator : IAgentGenerator
{
    private readonly ILogger<AgentGenerator> _logger;
    private readonly AgentConfig _agentConfig;

    public AgentGenerator(ILogger<AgentGenerator> logger, AgentConfig agentConfig)
    {
        _logger = logger;
        _agentConfig = agentConfig;
    }

    public IEnumerable<Agent> GenerateAgents(string teamName, int count, AgentSeniority seniority)
    {
        var agents = new List<Agent>();
        for (int i = 0; i < count; i++)
        {
            agents.Add(GenerateAgent(teamName, i + 1, seniority));
        }

        return agents;
    }

    private Agent GenerateAgent(string teamName, int count, AgentSeniority seniority)
    {
        var guid = Guid.NewGuid();
        var capacity = CalculateCapacity(seniority);

        _logger.LogInformation("Generating {Seniority} agent #{Count} for team {TeamName}", seniority, count, teamName);
        var name = $"{teamName} - {seniority} Agent #{count}";
        return new Agent(guid, name, seniority, capacity);
    }

    private int CalculateCapacity(AgentSeniority seniority) =>
        (int)Math.Floor(_agentConfig.MaximumConcurrentChats * seniority.GetMultiplier());
}