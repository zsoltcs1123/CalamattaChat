using Microsoft.AspNetCore.SignalR;
using SharedModels;
using SharedModels.Entities;

namespace ChatAPI.Services.Chat;

public class ChatSessionHub : Hub
{
    public async Task SendMessage(ChatSession chatSession)
    {
        await Clients.All.SendAsync("ReceiveChatSession", chatSession);
    }
}