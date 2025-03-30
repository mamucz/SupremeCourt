using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class CreateWaitingRoomHandler : IRequestHandler<CreateWaitingRoomCommand, WaitingRoomCreatedDto>
    {
        private readonly IWaitingRoomListService _waitingRoomService;
        private readonly ILogger<CreateWaitingRoomHandler> _logger;

        public CreateWaitingRoomHandler(
            IWaitingRoomListService waitingRoomService,
            ILogger<CreateWaitingRoomHandler> logger)
        {
            _waitingRoomService = waitingRoomService;
            _logger = logger;
        }

        public async Task<WaitingRoomCreatedDto> Handle(CreateWaitingRoomCommand request, CancellationToken cancellationToken)
        {
            if (request.PlayerId <= 0)
                return null!;

            var room = await _waitingRoomService.CreateWaitingRoomAsync(request.PlayerId);
            if (room == null) return null!;

            _logger.LogInformation("🎯 Waiting room {Id} vytvořena hráčem {PlayerId}", room.Id, room.CreatedByPlayerId);

            return new WaitingRoomCreatedDto
            {
                WaitingRoomId = room.Id,
                CreatedAt = room.CreatedAt,
                CreatedByPlayerId = room.CreatedByPlayerId
            };
        }
    }
}