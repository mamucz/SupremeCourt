using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class JoinWaitingRoomHandler : IRequestHandler<JoinWaitingRoomCommand, bool>
    {
        private readonly IWaitingRoomRepository _waitingRoomRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IGameService _gameService;
        private readonly IWaitingRoomNotifier _notifier;
        private readonly ILogger<JoinWaitingRoomHandler> _logger;

        public JoinWaitingRoomHandler(
            IWaitingRoomRepository waitingRoomRepository,
            IPlayerRepository playerRepository,
            IGameService gameService,
            IWaitingRoomNotifier notifier,
            ILogger<JoinWaitingRoomHandler> logger)
        {
            _waitingRoomRepository = waitingRoomRepository;
            _playerRepository = playerRepository;
            _gameService = gameService;
            _notifier = notifier;
            _logger = logger;
        }

        public async Task<bool> Handle(JoinWaitingRoomCommand request, CancellationToken cancellationToken)
        {
            var waitingRoom = await _waitingRoomRepository.GetByIdAsync(request.WaitingRoomId, cancellationToken);
            if (waitingRoom == null) return false;

            //if (waitingRoom.Players.Any(p => p.Id == request.PlayerId))
            //    return true; // už je připojený
            _logger.LogInformation("➡️ Kontrola hráčů v místnosti ID: {RoomId}", waitingRoom.Id);
            _logger.LogInformation("➡️ Počet hráčů: {Count}",  waitingRoom.Players.Count);
            
            _logger.LogInformation("🧪 Hledám hráče ID: {RequestId}", request.PlayerId);
            var alreadyJoined = waitingRoom.Players.Any(p => p.Id == request.PlayerId);
            _logger.LogInformation("Výsledek kontrola připojení: {Result}", alreadyJoined);


            if (waitingRoom.Players.Count >= GameRules.MaxPlayers)
            {
                _logger.LogWarning($"Hráč {request.PlayerId} se pokusil připojit do plné místnosti {request.WaitingRoomId}.");
                return false;
            }

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);
            if (player == null) return false;

            waitingRoom.Players.Add(player);
            await _waitingRoomRepository.UpdateAsync(waitingRoom, cancellationToken);

            await _notifier.NotifyPlayerJoinedAsync(request.WaitingRoomId, player.User?.Username ?? "Hráč");

            if (waitingRoom.Players.Count == GameRules.MaxPlayers)
            {
                _logger.LogInformation($"Místnost {request.WaitingRoomId} má 5 hráčů – spouštíme hru.");
                return await _gameService.StartGameAsync(request.WaitingRoomId);
            }

            return true;
        }
    }
}
