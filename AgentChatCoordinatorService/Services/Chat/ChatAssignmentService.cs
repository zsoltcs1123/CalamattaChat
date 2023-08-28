using AgentChatCoordinatorService.Models;
using AgentChatCoordinatorService.Services.Teams;
using SharedModels;
using SharedModels.Entities;
using SharedModels.Enums;

namespace AgentChatCoordinatorService.Services.Chat
{
    public class ChatAssignmentService : IChatAssignmentService
    {
        private readonly ILogger<ChatAssignmentService> _logger;
        private readonly ITeamService _teamService;

        private readonly Queue<ChatSession> _chatSessionQueue = new();

        public ChatAssignmentService(
            ILogger<ChatAssignmentService> logger,
            ITeamService teamService)
        {
            _logger = logger;
            _teamService = teamService;
        }

        public bool TryAssignChatToAgent(ChatSession chatSession, out Agent? agent)
        {
            var activeTeam = _teamService.GetActiveTeam();
            if (activeTeam == null)
            {
                _logger.LogError("No active team found");
                agent = null;
                return false;
            }

            if (AssignChatToAvailableAgent(chatSession, activeTeam, out agent))
            {
                return true;
            }

            if (_chatSessionQueue.Count <= activeTeam.MaximumQueueSize)
            {
                chatSession.Status = ChatSessionStatus.Queued;
                _chatSessionQueue.Enqueue(chatSession);
                return true;
            }

            return TryAssignChatToOverflowTeam(chatSession, out agent);
        }

        private bool AssignChatToAvailableAgent(ChatSession chatSession, Team team, out Agent? agent)
        {
            Agent? availableAgent = team.Agents
                .Where(agent => agent.CanTakeChat)
                .MinBy(agent => agent.Seniority);

            if (availableAgent == null)
            {
                agent = null;
                return false;
            }

            chatSession.Status = ChatSessionStatus.Assigned;
            agent = availableAgent;
            availableAgent.CurrentChats++;
            
            _logger.LogInformation("Assigned {TeamName} agent {@Agent} to chat session {@ChatSession}", team.Name,
                availableAgent, chatSession.Id);
            return true;
        }

        private bool TryAssignChatToOverflowTeam(ChatSession chatSession, out Agent? agent)
        {
            var overflowTeam = _teamService.GetOverflowTeam();
            if (overflowTeam == null)
            {
                _logger.LogError("No overflow team found");
                agent = null;
                return false;
            }

            if (AssignChatToAvailableAgent(chatSession, overflowTeam, out agent))
            {
                return true;
            }

            _logger.LogInformation("All overflow agents are busy");
            return false;
        }
    }
}
