using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Models;
using AgentChatCoordinatorService.Services.Agents;

namespace AgentChatCoordinatorService.Services.Teams;

public class TeamService : ITeamService
{
    private readonly ILogger<TeamService> _logger;
    private readonly IAgentGenerator _agentGenerator;
    private readonly IConfigurationSection _teamsConfigSection;
    public List<Team> Teams { get; }

    public TeamService(
        ILogger<TeamService> logger,
        IAgentGenerator agentGenerator,
        IConfiguration configuration)
    {
        _logger = logger;
        _agentGenerator = agentGenerator;
        _teamsConfigSection = configuration.GetSection("Teams");
        Teams = InitializeTeams();
    }
    
    public Team? GetActiveTeam()
    {
        return Teams.FirstOrDefault(t => t is { IsTeamOnShift: true, IsOverflowTeam: false });
    }

    public Team? GetOverflowTeam()
    {
        return Teams.FirstOrDefault(t => t is {IsOverflowTeam: true});
    }

    private List<Team> InitializeTeams()
    {
        var teams = new List<Team>();
        foreach (var teamConfigSection in _teamsConfigSection.GetChildren())
        {
            var teamConfig = teamConfigSection.Get<TeamConfig>();
            var teamName = teamConfigSection.Key;

            teams.Add(CreateTeamFromConfig(teamName, teamConfig));
        }

        return teams;
    }

    private Team CreateTeamFromConfig(string teamName, TeamConfig teamConfig)
    {
        var agents = new List<Agent>();

        //Add agents...
        agents.AddRange(_agentGenerator.GenerateAgents(teamName, teamConfig.TeamLead, AgentSeniority.TeamLead));
        agents.AddRange(_agentGenerator.GenerateAgents(teamName, teamConfig.Senior, AgentSeniority.Senior));
        agents.AddRange(_agentGenerator.GenerateAgents(teamName, teamConfig.MidLevel, AgentSeniority.MidLevel));
        agents.AddRange(_agentGenerator.GenerateAgents(teamName, teamConfig.Junior, AgentSeniority.Junior));

        TimeSpan? shiftStart = null;
        if (!string.IsNullOrEmpty(teamConfig.ShiftStart))
        {
            // Convert string to TimeSpan
            shiftStart = TimeSpan.Parse(teamConfig.ShiftStart);
        }

        var team = new Team(teamName, shiftStart, agents);
        
        _logger.LogInformation("{TeamName} has been created with the following details: {@Team}", teamName, team);

        return team;
    }
}