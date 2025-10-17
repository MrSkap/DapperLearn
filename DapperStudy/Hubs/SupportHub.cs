using System.Collections.Concurrent;
using DapperStudy.Models.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DapperStudy.Hubs;

[Authorize]
public class SupportHub: Hub
{
     private static readonly ConcurrentDictionary<string, UserConnection> Connections = 
        new();
     private static readonly ConcurrentDictionary<string, List<string>> SupportRooms = 
        new();

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Connections.TryRemove(Context.ConnectionId, out var user))
        {
            if (user.IsSupportAgent && SupportRooms.TryRemove(user.UserId, out var rooms))
            {
                await Clients.All.SendAsync("SupportAgentDisconnected", user.UserId);
            }
            else
            {
                await Clients.All.SendAsync("UserDisconnected", user.UserId);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterUser(string? userId = null, string? userName = null, bool? isSupportAgent = null)
    {
        var claimsPrincipal = Context.User;
        var resolvedUserId = userId ?? claimsPrincipal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Context.ConnectionId;
        var resolvedUserName = userName ?? claimsPrincipal?.Identity?.Name ?? "visitor";
        var isManagerRole = claimsPrincipal?.IsInRole("Manager") == true || claimsPrincipal?.IsInRole("manager") == true;
        var resolvedIsSupport = isSupportAgent ?? isManagerRole;

        var userConnection = new UserConnection
        {
            UserId = resolvedUserId,
            UserName = resolvedUserName,
            ConnectionId = Context.ConnectionId,
            IsSupportAgent = resolvedIsSupport
        };

        Connections[Context.ConnectionId] = userConnection;

        if (resolvedIsSupport)
        {
            SupportRooms[resolvedUserId] = new List<string>();
            await Clients.All.SendAsync("SupportAgentConnected", new { userId = resolvedUserId, userName = resolvedUserName });
        }
        else
        {
            await Clients.All.SendAsync("UserConnected", new { userId = resolvedUserId, userName = resolvedUserName });
        }
    }

    public async Task SendMessage(string message, string? targetUserId = null)
    {
        if (!Connections.TryGetValue(Context.ConnectionId, out var sender))
            return;

        var chatMessage = new ChatMessage
        {
            UserId = sender.UserId,
            UserName = sender.UserName,
            Message = message,
            IsSupportAgent = sender.IsSupportAgent
        };

        if (targetUserId != null)
        {
            // Личное сообщение
            var targetConnection = Connections.Values.FirstOrDefault(c => c.UserId == targetUserId);
            if (targetConnection != null)
            {
                chatMessage.ChatRoom = $"private_{sender.UserId}_{targetUserId}";
                await Clients.Client(targetConnection.ConnectionId)
                    .SendAsync("ReceiveMessage", chatMessage);
                await Clients.Caller.SendAsync("ReceiveMessage", chatMessage);
            }
        }
        else
        {
            // Общее сообщение
            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }
    }

    public async Task JoinSupportRoom(string clientUserId)
    {
        if (!Connections.TryGetValue(Context.ConnectionId, out var supportAgent) || !supportAgent.IsSupportAgent)
            return;

        if (!SupportRooms.ContainsKey(supportAgent.UserId))
            SupportRooms[supportAgent.UserId] = new List<string>();

        SupportRooms[supportAgent.UserId].Add(clientUserId);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"support_{clientUserId}");
        await Clients.Group($"support_{clientUserId}")
            .SendAsync("SupportJoined", new { supportAgentId = supportAgent.UserId, supportAgentName = supportAgent.UserName });
    }

    public async Task LeaveSupportRoom(string clientUserId)
    {
        if (!Connections.TryGetValue(Context.ConnectionId, out var supportAgent) || !supportAgent.IsSupportAgent)
            return;

        if (SupportRooms.TryGetValue(supportAgent.UserId, out var room))
            room.Remove(clientUserId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"support_{clientUserId}");
    }

    public List<UserConnection> GetConnectedUsers()
    {
        return Connections.Values.ToList();
    }

    public List<UserConnection> GetSupportAgents()
    {
        return Connections.Values.Where(c => c.IsSupportAgent).ToList();
    }
}