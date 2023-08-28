using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Models;
using AgentChatCoordinatorService.Services.Agents;

namespace AgentChatCoordinatorService.Services.Teams;

public class TeamGenerator : ITeamGenerator
{
    private const double CapacityMultiplier = 1.5;
    
    private readonly ILogger<TeamGenerator> _logger;
    private readonly IAgentGenerator _agentGenerator;
    private readonly TeamsConfig _teamsConfig;

    public TeamGenerator(
        ILogger<TeamGenerator> logger, 
        IAgentGenerator agentGenerator, 
        TeamsConfig teamsConfig)
    {
        _logger = logger;
        _agentGenerator = agentGenerator;
        _teamsConfig = teamsConfig;
    }
    
    public IEnumerable<Team> GenerateTeams()
    {
        return _teamsConfig.Teams.Select(GenerateTeam);
    }

    private Team GenerateTeam(TeamConfig teamConfig)
    {
        var agents = new List<Agent>();

        //Add agents...
        agents.AddRange(_agentGenerator.GenerateAgents(teamConfig.Name, teamConfig.TeamLead, AgentSeniority.TeamLead));
        agents.AddRange(_agentGenerator.GenerateAgents(teamConfig.Name, teamConfig.Senior, AgentSeniority.Senior));
        agents.AddRange(_agentGenerator.GenerateAgents(teamConfig.Name, teamConfig.MidLevel, AgentSeniority.MidLevel));
        agents.AddRange(_agentGenerator.GenerateAgents(teamConfig.Name, teamConfig.Junior, AgentSeniority.Junior));

        TimeSpan? shiftStart = null;
        if (!string.IsNullOrEmpty(teamConfig.ShiftStart))
        {
            // Convert string to TimeSpan
            shiftStart = TimeSpan.Parse(teamConfig.ShiftStart);
        }

        var name = teamConfig.Name;
        var capacity = agents.Sum(agent => agent.Capacity);
        var maximumQueueSize = (int)Math.Floor(capacity * CapacityMultiplier);
        var isOverflowTeam = shiftStart == null;

        var team = new Team(name, shiftStart, capacity, maximumQueueSize, isOverflowTeam, agents);

        _logger.LogInformation("{TeamName} has been created with the following details: {@Team}", teamConfig.Name,
            team);

        return team;
    }

}