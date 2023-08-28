using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Services.Teams;

public interface ITeamService
{
    List<Team> Teams { get; }

    Team? GetActiveTeam();
    
    Team? GetOverflowTeam();
}