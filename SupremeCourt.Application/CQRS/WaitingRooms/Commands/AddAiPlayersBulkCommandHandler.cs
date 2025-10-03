using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands;

/// <summary>
/// CQRS handler pro hromadné přidání AI hráčů do čekací místnosti.
/// Využívá jednotnou metodu <see cref="IWaitingRoomSessionManager.TryAddPlayerToRoomAsync"/>.
/// </summary>
public class AddAiPlayersBulkCommandHandler : IRequestHandler<AddAiPlayersBulkCommand, Unit>
{
    private readonly IWaitingRoomSessionManager _sessionManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AddAiPlayersBulkCommandHandler> _logger;

    public AddAiPlayersBulkCommandHandler(
        IWaitingRoomSessionManager sessionManager,
        IServiceScopeFactory scopeFactory,
        ILogger<AddAiPlayersBulkCommandHandler> logger)
    {
        _sessionManager = sessionManager;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<Unit> Handle(AddAiPlayersBulkCommand request, CancellationToken cancellationToken)
    {
        
            //using var scope = _scopeFactory.CreateScope();

            //var aiFactory = scope.ServiceProvider.GetRequiredService<IAiPlayerFactory>();
            //var playerRepo = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();

            //// Zajistí, že hráč s daným typem existuje v DB
            //await playerRepo.EnsureAiPlayerExistsAsync(request.Type, cancellationToken);

            //// Vytvoří instanci AI hráče
            //var aiPlayer = await aiFactory.CreateAsync(request.Type);

            //// Pokusí se přidat hráče do místnosti včetně SignalR notifikace
            //var success = await _sessionManager.TryAddPlayerToRoomAsync(request.WaitingRoomId, aiPlayer, cancellationToken);
            //if (!success)
            //{
            //    _logger.LogWarning("⚠️ Nepodařilo se přidat AI hráče {Index}/{Count} do místnosti {RoomId}.", i + 1, request.Count, request.WaitingRoomId);
            //    break;
        

        return Unit.Value;
    }
}
