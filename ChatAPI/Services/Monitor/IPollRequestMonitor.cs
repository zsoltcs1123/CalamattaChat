using SharedModels.Entities;

namespace ChatAPI.Services.Monitor;

public interface IPollRequestMonitor
{
    void RegisterChatSession(ChatSession chatSession);
    void ResetMissedPolls(ChatSession chatSession);
    IEnumerable<ChatSession> CheckMissedPolls();
}