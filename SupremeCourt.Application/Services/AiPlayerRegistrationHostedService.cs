using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupremeCourt.Application.Services;

public class AiPlayerRegistrationHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AiPlayerRegistrationHostedService> _logger;

    public AiPlayerRegistrationHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<AiPlayerRegistrationHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("▶️ Registrace AI hráčů při startu...");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var registrar = scope.ServiceProvider.GetRequiredService<RegistrAIPlayersService>();
            await registrar.RegisterAiPlayersAsync(cancellationToken);
            _logger.LogInformation("✅ AI hráči úspěšně registrováni.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Chyba při registraci AI hráčů.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}