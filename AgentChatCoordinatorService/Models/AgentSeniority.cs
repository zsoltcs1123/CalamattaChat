using AgentChatCoordinatorService.Attributes;

namespace AgentChatCoordinatorService.Models;

public enum AgentSeniority
{
    [SeniorityMultiplier(0.4)]
    Junior = 1,

    [SeniorityMultiplier(0.6)]
    MidLevel = 2,

    [SeniorityMultiplier(0.8)]
    Senior = 3,
    
    [SeniorityMultiplier(0.5)]
    TeamLead = 4
}
