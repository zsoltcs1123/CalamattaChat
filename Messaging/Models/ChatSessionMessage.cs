using SharedModels.Entities;

namespace Messaging.Models;

public class ChatSessionMessage
{
    public ChatSession ChatSession { get; set; }
    // Any other metadata...

    public ChatSessionMessage(ChatSession chatSession)
    {
        ChatSession = chatSession;
    }
}