using Microsoft.AspNetCore.SignalR;

namespace SupremeCourt.Infrastructure.Interfaces
{
    public interface ISignalRSender
    {
        Task SendToUserAsync<T>(IHubContext<T> hubContext, string userId, string method, object payload) where T : Hub;
        Task SendToGroupAsync<T>(IHubContext<T> hubContext, string group, string method, object payload) where T : Hub;
        Task SendToAllAsync<T>(IHubContext<T> hubContext, string method, object payload) where T : Hub;
    }
}