using AgentChatCoordinatorService.Extensions;

namespace AgentChatCoordinatorService.Models;

public record Agent (string Name, AgentSeniority Seniority)
{
    public Guid Id { get; } = Guid.NewGuid();
    public int CurrentChats { get; set; }
    public static int MaxConcurrency => 10;
    public int Capacity => (int)Math.Floor(MaxConcurrency * Seniority.GetMultiplier());
    public bool CanTakeChat => CurrentChats < Capacity;
}
