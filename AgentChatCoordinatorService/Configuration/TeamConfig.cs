namespace AgentChatCoordinatorService.Configuration;

public class TeamConfig : IConfig
{
    public string Name { get; set; }
    public string ShiftStart { get; set; }
    public int TeamLead { get; set; }
    public int Senior { get; set; }
    public int MidLevel { get; set; }
    public int Junior { get; set; }


    public void Validate()
    {
        if (TeamLead == 0 && Senior == 0 && MidLevel == 0 && Junior == 0)
        {
            throw new InvalidOperationException("Invalid team configuration");
        }
    }
}