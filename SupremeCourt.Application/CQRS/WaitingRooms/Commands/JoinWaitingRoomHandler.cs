using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Application.CQRS.WaitingRooms.Commands;
using SupremeCourt.Domain.Interfaces;

public class JoinWaitingRoomHandler : IRequestHandler<JoinWaitingRoomCommand, bool>
{
    private readonly IWaitingRoomSessionManager _sessionManager;
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<JoinWaitingRoomHandler> _logger;

    public JoinWaitingRoomHandler(
        IWaitingRoomSessionManager sessionManager,
        IPlayerRepository playerRepository,
        IGameService gameService, // nepoužitý, ale můžeš ho zatím ponechat
        IWaitingRoomNotifier notifier, // taky už není třeba zde
        ILogger<JoinWaitingRoomHandler> logger)
    {
        _sessionManager = sessionManager;
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(JoinWaitingRoomCommand request, CancellationToken cancellationToken)
    {
        var player = await _playerRepository.GetByIdAsync(request.PlayerId);
        if (player == null)
        {
            _logger.LogWarning("❌ Hráč {PlayerId} nebyl nalezen v databázi.", request.PlayerId);
            return false;
        }

        // Použij sjednocenou logiku včetně SignalR notifikací
        var success = await _sessionManager.TryAddPlayerToRoomAsync(request.WaitingRoomId, player, cancellationToken);

        if (!success)
        {
            _logger.LogWarning("❌ Připojení hráče {PlayerId} do místnosti {RoomId} selhalo.", request.PlayerId, request.WaitingRoomId);
            return false;
        }

        return true;
    }
}