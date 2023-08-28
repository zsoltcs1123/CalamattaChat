using AgentChatCoordinatorService.Extensions;

namespace AgentChatCoordinatorService.Models;

public record Agent(string Name, AgentSeniority Seniority)
{
    private const double MaxConcurrency = 10;

    public Guid Id { get; } = Guid.NewGuid();
    public int CurrentChats { get; set; }
    public int Capacity => (int)Math.Floor(MaxConcurrency * Seniority.GetMultiplier());
    public bool CanTakeChat => CurrentChats < Capacity;
}