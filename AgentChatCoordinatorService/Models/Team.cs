namespace AgentChatCoordinatorService.Models;

public record Team(
    string Name, 
    TimeSpan? ShiftStart, 
    int Capacity, 
    int MaximumQueueSize, 
    bool IsOverflowTeam,
    List<Agent> Agents)
{
    public bool IsTeamOnShift
    {
        get
        {
            if (IsOverflowTeam)
            {
                return true;
            }

            var currentTime = DateTime.Now.TimeOfDay;
            var shiftEnd = ShiftStart?.Add(new TimeSpan(8, 0, 0)); // 8 hours after shift start

            // Check if the current time is within the team's shift time
            return currentTime >= ShiftStart && currentTime < shiftEnd;
        }
    }
}