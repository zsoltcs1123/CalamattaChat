using ChatAPI.Configuration;
using ChatAPI.Services.Monitor;
using Messaging.Models;
using Messaging.Services.Chat;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Entities;

namespace ChatAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IChatSessionPublisher _chatSessionPublisher;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private readonly IPollRequestMonitor _pollRequestMonitor;

    public ChatController(
        ILogger<ChatController> logger,
        IChatSessionPublisher chatSessionPublisher,
        RabbitMQConfig rabbitMQConfig,
        IPollRequestMonitor pollRequestMonitor)
    {
        _logger = logger;
        _chatSessionPublisher = chatSessionPublisher;
        _rabbitMQConfig = rabbitMQConfig;
        _pollRequestMonitor = pollRequestMonitor;

    }

    [HttpPost("initiate")]
    public IActionResult InitiateChat()
    {
        var chatSession = new ChatSession(Guid.NewGuid(), DateTime.Now);

        _logger.LogInformation("ChatSession {@ChatSession} initiated ", chatSession);

        _pollRequestMonitor.RegisterChatSession(chatSession);
        
        _chatSessionPublisher.Publish(new ChatSessionMessage(chatSession), _rabbitMQConfig.ChatSessionQueueName);

        return Ok();
    }

    [HttpPost("poll")]
    public IActionResult Poll([FromBody] ChatSession chatSession)
    {
        if (chatSession.Id == Guid.Empty)
        {
            return BadRequest("Invalid chat session provided.");
        }

        _pollRequestMonitor.ResetMissedPolls(chatSession);
        return Ok();
    }
}