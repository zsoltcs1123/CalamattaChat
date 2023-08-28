using System.Collections.Concurrent;
using SharedModels.Entities;

namespace ChatAPI.Services.Monitor;

public class PollRequestMonitor : IPollRequestMonitor
{
    private readonly ILogger<PollRequestMonitor> _logger;

    private readonly ConcurrentDictionary<Guid, int> _missedPolls = new();
    private readonly ConcurrentDictionary<Guid, ChatSession> _chatSessions = new();

    public PollRequestMonitor(ILogger<PollRequestMonitor> logger)
    {
        _logger = logger;
    }

    public void RegisterChatSession(ChatSession chatSession)
    {
        _logger.LogInformation("ChatSession {ChatSession} registered", chatSession);
        _chatSessions[chatSession.Id] = chatSession;
        _missedPolls[chatSession.Id] = 0;
    }

    public void ResetMissedPolls(ChatSession chatSession)
    {
        //_logger.LogInformation("Missed poll count reset for id {Guid}", sessionId);
        _missedPolls[chatSession.Id] = 0;
    }

    public IEnumerable<ChatSession> CheckMissedPolls()
    {
        var inactiveSessions = new List<ChatSession>();
        foreach (var id in _missedPolls.Keys)
        {
            _missedPolls.AddOrUpdate(id, 1, (key, oldValue) => oldValue + 1);

            if (_missedPolls[id] >= 3)
            {
                _logger.LogInformation("Missed poll count reached threshold for id {@ChatSession}", id);

                _missedPolls.TryRemove(id, out _);

                if (_chatSessions.TryGetValue(id, out var chatSession))
                {
                    inactiveSessions.Add(chatSession);
                }

                _chatSessions.TryRemove(id, out _);
            }
        }

        return inactiveSessions;
    }
}