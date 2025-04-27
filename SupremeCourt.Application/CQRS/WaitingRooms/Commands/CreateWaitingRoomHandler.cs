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
            {
                _logger.LogWarning("❌ Neplatné PlayerId: {PlayerId}", request.PlayerId);
                return null!;
            }

            var room = await _waitingRoomService.CreateWaitingRoomAsync(request.PlayerId, cancellationToken);
            if (room == null)
            {
                _logger.LogWarning("❌ Nepodařilo se vytvořit místnost pro hráče {PlayerId}", request.PlayerId);
                return null!;
            }

            _logger.LogInformation("🎯 Waiting room {RoomId} vytvořena hráčem {PlayerId}", room.WaitingRoomId, room.CreatedBy.Id);

            return new WaitingRoomCreatedDto
            {
                WaitingRoomId = room.WaitingRoomId,
                CreatedAt = room.CreatedAt,
                CreatedByPlayerId = room.CreatedBy.Id
            };
        }
    }
}