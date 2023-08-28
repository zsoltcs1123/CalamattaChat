using AgentChatCoordinatorService.Models;
using SharedModels.Entities;

namespace AgentChatCoordinatorService.Services.Chat;

public interface IChatAssignmentService
{
    bool TryAssignChatToAgent(ChatSession chatSession, out Agent? agent);
}