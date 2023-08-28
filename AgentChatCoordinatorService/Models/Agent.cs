namespace AgentChatCoordinatorService.Models;

public class Agent
{
    public Guid Id { get; }
    public string Name { get; }
    public AgentSeniority Seniority { get; }
    public int Capacity { get; }
    public int CurrentChats { get; set; }
    public bool CanTakeChat => CurrentChats < Capacity;

    public Agent(Guid id, string name, AgentSeniority seniority, int capacity)
    {
        Id = id;
        Name = name;
        Seniority = seniority;
        Capacity = capacity;
    }
}