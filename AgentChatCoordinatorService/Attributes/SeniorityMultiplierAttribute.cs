namespace AgentChatCoordinatorService.Attributes;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class SeniorityMultiplierAttribute : Attribute
{
    public double Multiplier { get; }

    public SeniorityMultiplierAttribute(double multiplier)
    {
        Multiplier = multiplier;
    }
}
