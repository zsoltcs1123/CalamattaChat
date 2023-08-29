namespace AgentChatCoordinatorService.Services.Time;

public class TimeService : ITimeService
{
    public TimeSpan GetCurrentTimeOfDay()
    {
        return DateTime.Now.TimeOfDay;
    }
}