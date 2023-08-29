namespace AgentChatCoordinatorService.Configuration;

public class TeamsConfig : IConfig
{
    public List<TeamConfig> Teams { get; }

    public TeamsConfig(List<TeamConfig> teams)
    {
        Teams = teams;
    }

    public void Validate()
    {
        if (Teams.Count == 0)
        {
            throw new InvalidOperationException("No teams configured");
        }
    }
}