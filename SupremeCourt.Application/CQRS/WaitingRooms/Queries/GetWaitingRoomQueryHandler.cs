using MediatR;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Queries
{
    public class GetWaitingRoomQueryHandler : IRequestHandler<GetWaitingRoomQuery, WaitingRoomDto?>
    {
        private readonly IWaitingRoomRepository _waitingRoomRepository;
        private readonly IPlayerRepository _playerRepository;

        public GetWaitingRoomQueryHandler(IWaitingRoomRepository waitingRoomRepository, IPlayerRepository playerRepository)
        {
            _waitingRoomRepository = waitingRoomRepository;
            _playerRepository = playerRepository;
        }

        public async Task<WaitingRoomDto?> Handle(GetWaitingRoomQuery request, CancellationToken cancellationToken)
        {
            var room = await _waitingRoomRepository.GetByIdAsync(request.WaitingRoomId);
            if (room == null)
                return null;

            var players = room.Players.Select(p => new PlayerDto
            {
                PlayerId = p.Id,
                Username = p.User?.Username ?? "Unknown"
            }).ToList();

            var dto = new WaitingRoomDto
            {
                WaitingRoomId = room.Id,
                CreatedAt = room.CreatedAt,
                CreatedByPlayerId = room.CreatedByPlayerId,
                Players = players
            };

            return dto;
        }
    }
}