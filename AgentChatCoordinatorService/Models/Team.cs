namespace AgentChatCoordinatorService.Models;

public record Team(string Name, TimeSpan? ShiftStart, List<Agent> Agents)
{
    private const double CapacityMultiplier = 1.5;

    public int Capacity
    {
        get
        {
            return Agents.Sum(agent => agent.Capacity);
        }
    }

    public int MaximumQueueSize => (int)Math.Floor(Capacity * CapacityMultiplier);

    public bool IsOverflowTeam => ShiftStart == null;

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