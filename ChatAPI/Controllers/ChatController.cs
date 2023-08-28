using ChatAPI.Configuration;
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

    public ChatController(
        ILogger<ChatController> logger,
        IChatSessionPublisher chatSessionPublisher,
        RabbitMQConfig rabbitMQConfig)
    {
        _logger = logger;
        _chatSessionPublisher = chatSessionPublisher;
        _rabbitMQConfig = rabbitMQConfig;

    }

    [HttpPost("initiate")]
    public IActionResult InitiateChat()
    {
        var chatSession = new ChatSession(Guid.NewGuid(), DateTime.Now);

        _logger.LogInformation("ChatSession {@ChatSession} initiated ", chatSession);

        _chatSessionPublisher.Publish(new ChatSessionMessage(chatSession), _rabbitMQConfig.ChatSessionQueueName);

        return Ok();
    }
}