using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HRM.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinChatRoom(string chatRoomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
        await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("UserJoined", Context.User?.Identity?.Name);
    }

    public async Task LeaveChatRoom(string chatRoomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
        await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("UserLeft", Context.User?.Identity?.Name);
    }

    public async Task SendMessage(string chatRoomId, string message)
    {
        var username = Context.User?.Identity?.Name ?? "Unknown";
        var employeeId = Context.User?.FindFirst("EmployeeId")?.Value;

        await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("ReceiveMessage", new
        {
            SenderId = employeeId,
            SenderName = username,
            Content = message,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task TypingIndicator(string chatRoomId, bool isTyping)
    {
        var username = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.OthersInGroup($"ChatRoom_{chatRoomId}").SendAsync("UserTyping", username, isTyping);
    }

    public async Task MarkAsRead(string chatRoomId, int messageId)
    {
        await Clients.Group($"ChatRoom_{chatRoomId}").SendAsync("MessageRead", messageId);
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.All.SendAsync("UserConnected", username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name ?? "Unknown";
        await Clients.All.SendAsync("UserDisconnected", username);
        await base.OnDisconnectedAsync(exception);
    }
}
