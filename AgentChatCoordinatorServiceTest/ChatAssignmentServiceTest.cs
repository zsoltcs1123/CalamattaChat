using AgentChatCoordinatorService.Models;
using AgentChatCoordinatorService.Services.Chat;
using AgentChatCoordinatorService.Services.Teams;
using Microsoft.Extensions.Logging;
using Moq;
using SharedModels.Entities;

namespace AgentChatCoordinatorServiceTest;

[TestFixture]
public class ChatAssignmentServiceTests
{
    private Mock<ITeamService> _mockTeamService;
    private Mock<ILogger<ChatAssignmentService>> _mockLogger;
    private ChatAssignmentService _service;

    [SetUp]
    public void SetUp()
    {
        _mockTeamService = new Mock<ITeamService>();
        _mockLogger = new Mock<ILogger<ChatAssignmentService>>();
        _service = new ChatAssignmentService(_mockLogger.Object, _mockTeamService.Object);
    }
    
    private ChatSession CreateChatSession()
    {
        return new ChatSession(Guid.NewGuid(), DateTime.Now);
    }

    [Test]
    public void TryAssignChatToAgent_NoActiveTeam_ReturnsFalse()
    {
        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns((Team)null);

        var result = _service.TryAssignChatToAgent(CreateChatSession(), out var agent);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(agent, Is.Null);
        });
    }

    [Test]
    public void TryAssignChatToAgent_ActiveTeamNoAvailableAgent_ReturnsFalseIfQueueFull()
    {
        var team = new Team
        {
            Agents = new List<Agent> { new Agent { CanTakeChat = false } },
            MaximumQueueSize = 0 // Ensure the queue is "full"
        };
        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(team);

        var result = _service.TryAssignChatToAgent(new ChatSession(), out var agent);

        Assert.IsFalse(result);
        Assert.IsNull(agent);
    }

    [Test]
    public void TryAssignChatToAgent_ActiveTeamAvailableAgent_AssignsToAgentWithLowestSeniority()
    {
        var team = new Team
        {
            Agents = new List<Agent>
            {
                new Agent { CanTakeChat = true, Seniority = 2 },
                new Agent { CanTakeChat = true, Seniority = 1 }
            }
        };
        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(team);

        var result = _service.TryAssignChatToAgent(new ChatSession(), out var agent);

        Assert.IsTrue(result);
        Assert.AreEqual(1, agent.Seniority);
    }

    [Test]
    public void TryAssignChatToAgent_QueueFullNoOverflowTeam_ReturnsFalse()
    {
        var team = new Team
        {
            Agents = new List<Agent> { new Agent { CanTakeChat = false } },
            MaximumQueueSize = 0
        };
        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(team);
        _mockTeamService.Setup(ts => ts.GetOverflowTeam()).Returns((Team)null);

        var result = _service.TryAssignChatToAgent(new ChatSession(), out var agent);

        Assert.IsFalse(result);
        Assert.IsNull(agent);
    }

    [Test]
    public void TryAssignChatToAgent_OverflowTeamNoAvailableAgent_ReturnsFalse()
    {
        var activeTeam = new Team
        {
            Agents = new List<Agent> { new Agent { CanTakeChat = false } },
            MaximumQueueSize = 0
        };
        var overflowTeam = new Team
        {
            Agents = new List<Agent> { new Agent { CanTakeChat = false } }
        };
        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(activeTeam);
        _mockTeamService.Setup(ts => ts.GetOverflowTeam()).Returns(overflowTeam);

        var result = _service.TryAssignChatToAgent(new ChatSession(), out var agent);

        Assert.IsFalse(result);
        Assert.IsNull(agent);
    }

    [Test]
    public void TryAssignChatToAgent_OverflowTeamAvailableAgent_AssignsToAgentWithLowestSeniority()
    {
        var activeTeam = new Team
        {
            Agents = new List<Agent> { new Agent { CanTakeChat = false } },
            MaximumQueueSize = 0
        };
        var overflowTeam = new Team
        {
            Agents = new List<Agent>
            {
                new Agent { CanTakeChat = true, Seniority = 2 },
                new Agent { CanTakeChat = true, Seniority = 1 }
            }
        };
        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(activeTeam);
        _mockTeamService.Setup(ts => ts.GetOverflowTeam()).Returns(overflowTeam);

        var result = _service.TryAssignChatToAgent(new ChatSession(), out var agent);

        Assert.IsTrue(result);
        Assert.AreEqual(1, agent.Seniority);
    }
}