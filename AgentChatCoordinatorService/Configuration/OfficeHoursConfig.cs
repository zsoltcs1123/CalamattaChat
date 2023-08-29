namespace AgentChatCoordinatorService.Configuration;

public class OfficeHoursConfig : IConfig
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    
    public void Validate()
    {
        if (Start == default || End == default || End < Start)
        {
            throw new InvalidOperationException("Office hours must be valid");
        }
    }
    
    public bool IsOfficeHours()
    {
        var currentTime = DateTime.Now.TimeOfDay;
        return currentTime >= Start && currentTime <= End;
    }

}