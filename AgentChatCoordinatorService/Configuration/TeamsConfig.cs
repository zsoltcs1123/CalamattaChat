namespace AgentChatCoordinatorService.Configuration;

public class TeamsConfig
{
    public List<TeamConfig> Teams { get; }

    public TeamsConfig(List<TeamConfig> teams)
    {
        Teams = teams;
    }

    public bool Validate()
    {
        return Teams.Count > 0;
    }
}