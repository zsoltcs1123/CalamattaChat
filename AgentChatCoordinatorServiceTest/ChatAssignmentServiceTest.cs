using AgentChatCoordinatorService.Configuration;
using AgentChatCoordinatorService.Models;
using AgentChatCoordinatorService.Services.Chat;
using AgentChatCoordinatorService.Services.Teams;
using AgentChatCoordinatorService.Services.Time;
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
    private OfficeHoursConfig officeHoursConfig;

    [SetUp]
    public void SetUp()
    {
        _mockTeamService = new Mock<ITeamService>();
        _mockLogger = new Mock<ILogger<ChatAssignmentService>>();
        officeHoursConfig = new OfficeHoursConfig
        {
            Start = new TimeSpan(9, 0, 0), 
            End = new TimeSpan(17, 0, 0)
        };
        var timeServiceMock = new Mock<ITimeService>();
        timeServiceMock.Setup(ts => ts.GetCurrentTimeOfDay()).Returns(new TimeSpan(12, 0, 0));
        _service = new ChatAssignmentService(_mockLogger.Object, _mockTeamService.Object, officeHoursConfig, timeServiceMock.Object);
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
        var agent = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Senior, 1);

        var team = new Team(
            "",
            DateTime.Now.TimeOfDay,
            1,
            1,
            false,
            new List<Agent> { agent });

        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(team);

        _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent1);
        _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent2);
        
        var result = _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent3);


        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(assignedAgent3, Is.Null);
        });
    }

    [Test]
    public void TryAssignChatToAgent_ActiveTeamAvailableAgent_AssignsToAgentWithLowestSeniority()
    {
        var juniorAgent = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Junior, 10);
        var seniorAgent = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Senior, 10);

        var team = new Team(
            "",
            DateTime.Now.TimeOfDay,
            10,
            10,
            false,
            new List<Agent> { juniorAgent, seniorAgent });

        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(team);

        var result = _service.TryAssignChatToAgent(CreateChatSession(), out var agent);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(agent, Is.EqualTo(juniorAgent));
        });
    }

    [Test]
    public void TryAssignChatToAgent_QueueFullNoOverflowTeam_ReturnsFalse()
    {
        var agent = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Senior, 1);

        var team = new Team(
            "",
            DateTime.Now.TimeOfDay,
            1,
            0,
            false,
            new List<Agent> { agent });

        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(team);
        _mockTeamService.Setup(ts => ts.GetOverflowTeam()).Returns((Team)null);

        _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent1);

        var result = _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent3);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(assignedAgent3, Is.Null);
        });
    }

    [Test]
    public void TryAssignChatToAgent_OverflowTeamNoAvailableAgent_ReturnsFalse()
    {
        var agent1 = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Senior, 1);
        var agent2 = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Senior, 1);

        var activeTeam = new Team(
            "",
            DateTime.Now.TimeOfDay,
            1,
            0,
            false,
            new List<Agent> { agent1 });


        var overflowTeam = new Team(
            "",
            DateTime.Now.TimeOfDay,
            1,
            0,
            true,
            new List<Agent> { agent2 });

        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(activeTeam);
        _mockTeamService.Setup(ts => ts.GetOverflowTeam()).Returns(overflowTeam);

        _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent1);
        _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent2);

        var result = _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent3);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.False);
            Assert.That(assignedAgent3, Is.Null);
        });
    }

    [Test]
    public void TryAssignChatToAgent_OverflowTeamAvailableAgent_AssignsToAgentWithLowestSeniority()
    {
        var agent1 = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Senior, 1);
        var agent2 = new Agent(Guid.NewGuid(), string.Empty, AgentSeniority.Senior, 2);

        var activeTeam = new Team(
            "",
            DateTime.Now.TimeOfDay,
            1,
            0,
            false,
            new List<Agent> { agent1 });


        var overflowTeam = new Team(
            "",
            DateTime.Now.TimeOfDay,
            1,
            0,
            true,
            new List<Agent> { agent2 });

        _mockTeamService.Setup(ts => ts.GetActiveTeam()).Returns(activeTeam);
        _mockTeamService.Setup(ts => ts.GetOverflowTeam()).Returns(overflowTeam);

        _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent1);
        _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent2);

        var result = _service.TryAssignChatToAgent(CreateChatSession(), out var assignedAgent3);
       
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(assignedAgent3, Is.EqualTo(agent2));
        });
    }
}