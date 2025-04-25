using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;
using System;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class AddAiPlayerToRoomCommandHandler : IRequestHandler<AddAiPlayerToRoomCommand, bool>
    {
        private readonly WaitingRoomSessionManager _sessionManager;
        private readonly IWaitingRoomEventHandler _eventHandler;
        private readonly ILogger<AddAiPlayerToRoomCommandHandler> _logger;
        public AddAiPlayerToRoomCommandHandler(
            WaitingRoomSessionManager sessionManager,
            IWaitingRoomEventHandler eventHandler,
            ILogger<AddAiPlayerToRoomCommandHandler> logger)
        {
            _sessionManager = sessionManager;
            _eventHandler = eventHandler;
            _logger = logger;
        }

        public async Task<bool> Handle(AddAiPlayerToRoomCommand request, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(request.WaitingRoomId);
            if (session == null)
            {
                _logger.LogWarning("❌ AI hráč: Místnost {RoomId} neexistuje", request.WaitingRoomId);
                return false;
            }

            if (session.Players.Count >= 5)
            {
                _logger.LogWarning("❌ AI hráč: Místnost {RoomId} je plná", request.WaitingRoomId);
                return false;
            }


            session.Players.Add(request.player);

            await _eventHandler.NotifyPlayerJoinedAsync(request.WaitingRoomId, Domain.Mappings.PlayerMapper.Instance.ToDto(request.player as Player), cancellationToken);
            _logger.LogInformation("🤖 AI hráč {Username} přidán do místnosti {RoomId}", request.player.Username, request.WaitingRoomId);

            return true;
        }

        private int GenerateFakeId()
        {
            return -1 * new Random().Next(1, 100000); // záporná ID pro odlišení
        }

        private string GenerateRandomAiName()
        {
            var rnd = new Random().Next(1000, 9999);
            return $"AI_{rnd}";
        }
    }
}
