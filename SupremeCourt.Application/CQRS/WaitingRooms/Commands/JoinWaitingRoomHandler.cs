using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Application.CQRS.WaitingRooms.Commands;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

public class JoinWaitingRoomHandler : IRequestHandler<JoinWaitingRoomCommand, bool>
{
    private readonly IWaitingRoomSessionManager _sessionManager;
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<JoinWaitingRoomHandler> _logger;

    public JoinWaitingRoomHandler(
        IWaitingRoomSessionManager sessionManager,
        IPlayerRepository playerRepository,
        IGameService gameService, // nepoužitý
        IWaitingRoomNotifier notifier, // nepoužitý
        ILogger<JoinWaitingRoomHandler> logger)
    {
        _sessionManager = sessionManager;
        _playerRepository = playerRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(JoinWaitingRoomCommand request, CancellationToken cancellationToken)
    {
        var player =  _playerRepository.GetById(request.Player.Id);
        if (player == null)
        {
            _logger.LogWarning("❌ Hráč s ID {PlayerId} nebyl nalezen.", request.Player.Id);
            return false;
        }

        var success =  _sessionManager.AddPlayerToRoomAsync(request.WaitingRoomId, player);

        if (!success)
        {
            _logger.LogWarning("❌ Připojení hráče {Player} do místnosti {RoomId} selhalo.", request.Player.User.Username, request.WaitingRoomId);
            return false;
        }

        return true;
    }
}