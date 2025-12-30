using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HRM.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(string userId, string message, string type)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            Message = message,
            Type = type,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendNotificationToRole(string role, string message, string type)
    {
        await Clients.Group($"Role_{role}").SendAsync("ReceiveNotification", new
        {
            Message = message,
            Type = type,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendLeaveRequestNotification(string message)
    {
        // Send to HR and Admin
        await Clients.Groups("Role_HR", "Role_Admin").SendAsync("LeaveRequestNotification", new
        {
            Message = message,
            Type = "LeaveRequest",
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendPayrollNotification(string employeeId, string message)
    {
        await Clients.User(employeeId).SendAsync("PayrollNotification", new
        {
            Message = message,
            Type = "Payroll",
            Timestamp = DateTime.UtcNow
        });
    }

    public override async Task OnConnectedAsync()
    {
        var role = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (!string.IsNullOrEmpty(role))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var role = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (!string.IsNullOrEmpty(role))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Role_{role}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
