using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Services.Teams;

public class TeamService : ITeamService
{
    private readonly ILogger<TeamService> _logger;
    private readonly IConfigurationSection _teamsConfigSection;
    public List<Team> Teams { get; }

    public TeamService(
        ILogger<TeamService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
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
        agents.AddRange(GenerateAgents(teamName, teamConfig.TeamLead, AgentSeniority.TeamLead));
        agents.AddRange(GenerateAgents(teamName, teamConfig.Senior, AgentSeniority.Senior));
        agents.AddRange(GenerateAgents(teamName, teamConfig.MidLevel, AgentSeniority.MidLevel));
        agents.AddRange(GenerateAgents(teamName, teamConfig.Junior, AgentSeniority.Junior));

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

    private IEnumerable<Agent> GenerateAgents(string teamName, int count, AgentSeniority seniority)
    {
        var agents = new List<Agent>();
        for (int i = 0; i < count; i++)
        {
            agents.Add(GenerateAgent(teamName, i+1, seniority));
        }

        return agents;
    }

    private Agent GenerateAgent(string teamName, int count, AgentSeniority seniority)
    {
        _logger.LogInformation("Generating {Seniority} agent #{Count} for team {TeamName}", seniority, count, teamName);
        var name = $"{teamName} - {seniority} Agent #{count}";
        return new Agent(name, seniority);
    }
}