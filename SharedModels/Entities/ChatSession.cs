using SharedModels.Enums;

namespace SharedModels.Entities;

public class ChatSession
{
    public Guid Id { get; }
    public DateTime InitiatedAt { get; }
    public ChatSessionStatus Status { get; set; } 

    public ChatSession(Guid id, DateTime initiatedAt)
    {
        Id = id;
        InitiatedAt = initiatedAt;
        Status = ChatSessionStatus.Initiated;
    }

    public override string ToString()
    {
        return $"[{nameof(Id)}: {Id}, {nameof(InitiatedAt)}: {InitiatedAt}, {nameof(Status)}: {Status}]";
    }
}