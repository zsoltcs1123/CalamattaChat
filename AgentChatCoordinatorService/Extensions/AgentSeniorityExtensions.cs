using AgentChatCoordinatorService.Attributes;
using AgentChatCoordinatorService.Models;

namespace AgentChatCoordinatorService.Extensions;

public static class AgentSeniorityExtensions
{
    public static double GetMultiplier(this AgentSeniority seniority)
    {
        var type = typeof(AgentSeniority);
        var memInfo = type.GetMember(seniority.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(SeniorityMultiplierAttribute), false);
        return ((SeniorityMultiplierAttribute)attributes[0]).Multiplier;
    }
}
