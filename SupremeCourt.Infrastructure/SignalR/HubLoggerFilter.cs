using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SupremeCourt.Infrastructure.SignalR
{


    public class HubLoggerFilter : IHubFilter
    {
        private readonly ILogger<HubLoggerFilter> _logger;

        public HubLoggerFilter(ILogger<HubLoggerFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            _logger.LogInformation("➡️  SignalR volání: {Hub}.{Method}({Arguments})",
                invocationContext.Hub.GetType().Name,
                invocationContext.HubMethodName,
                string.Join(", ", invocationContext.HubMethodArguments));

            var result = await next(invocationContext);

            _logger.LogInformation("⬅️  Výsledek: {Result}", result);

            return result;
        }

        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            _logger.LogInformation("🔌 Klient připojen: {ConnectionId}", context.Context.ConnectionId);
            await next(context);
        }

        public async Task OnDisconnectedAsync(HubLifetimeContext context, Exception? exception,
            Func<HubLifetimeContext, Exception?, Task> next)
        {
            _logger.LogInformation("🔌 Klient odpojen: {ConnectionId}", context.Context.ConnectionId);

            if (exception != null)
                _logger.LogWarning(exception, "❗ Odpojení kvůli chybě");

            await next(context, exception);
        }
    }
}