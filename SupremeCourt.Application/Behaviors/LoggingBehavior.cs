using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Application.Behaviors
{ 
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("➡️ Handling {RequestName} with content: {@Request}", requestName, request);

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("✅ {RequestName} handled in {ElapsedMilliseconds}ms with response: {@Response}",
                    requestName, stopwatch.ElapsedMilliseconds, response);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ {RequestName} failed in {ElapsedMilliseconds}ms", requestName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}