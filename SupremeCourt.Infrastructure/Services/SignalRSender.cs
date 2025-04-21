using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Infrastructure.Interfaces;

public class SignalRSender : ISignalRSender
{
    private readonly ILogger<SignalRSender> _logger;

    public SignalRSender(ILogger<SignalRSender> logger)
    {
        _logger = logger;
    }

    public async Task SendToUserAsync<T>(IHubContext<T> hubContext, string userId, string method, object payload) where T : Hub
    {
        _logger.LogInformation("SignalR -> User: {UserId} | Method: {Method} | Payload: {@Payload}", userId, method, payload);
        await hubContext.Clients.User(userId).SendAsync(method, payload);
    }

    public async Task SendToGroupAsync<T>(IHubContext<T> hubContext, string group, string method, object payload) where T : Hub
    {
        _logger.LogInformation("SignalR -> Group: {Group} | Method: {Method} | Payload: {@Payload}", group, method, payload);
        await hubContext.Clients.Group(group).SendAsync(method, payload);
    }

    public async Task SendToAllAsync<T>(IHubContext<T> hubContext, string method, object payload) where T : Hub
    {
        _logger.LogInformation("SignalR -> All | Method: {Method} | Payload: {@Payload}", method, payload);
        await hubContext.Clients.All.SendAsync(method, payload);
    }
}
