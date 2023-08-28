using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Services.Teams;

public interface ITeamGenerator
{
    IEnumerable<Team> GenerateTeams();
}