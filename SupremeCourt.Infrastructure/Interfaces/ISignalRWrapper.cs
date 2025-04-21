using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Infrastructure.Interfaces
{
    public interface ISignalRWrapper
    {
        Task SendToUserAsync<T>(string userId, string method, T message);
        Task SendToGroupAsync<T>(string groupName, string method, T message);
        Task BroadcastAsync<T>(string method, T message);
    }
}
