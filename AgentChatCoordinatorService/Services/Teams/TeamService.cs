using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Services.Teams;

public class TeamService : ITeamService
{
    public List<Team> Teams { get; }

    public TeamService(ITeamGenerator teamGenerator)
    {
        Teams = teamGenerator.GenerateTeams().ToList();
    }

    public Team? GetActiveTeam()
    {
        return Teams.FirstOrDefault(t => t is { IsTeamOnShift: true, IsOverflowTeam: false });
    }

    public Team? GetOverflowTeam()
    {
        return Teams.FirstOrDefault(t => t is { IsOverflowTeam: true });
    }
}