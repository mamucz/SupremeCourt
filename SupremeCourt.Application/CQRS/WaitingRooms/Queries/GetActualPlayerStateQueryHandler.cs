using MediatR;
using SupremeCourt.Application.Common.Interfaces;
using SupremeCourt.Application.CQRS.WaitingRooms.Queries;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Handlers
{
    public class GetActualPlayerStateQueryHandler : IRequestHandler<GetActualPlayerStateQuery, ActualPlayerStateDto>
    {
        private readonly IWaitingRoomService _waitingRoomService;
        private readonly IGameService _gameService;
        private readonly IUserSessionRepository _userSessionRepository;

        public GetActualPlayerStateQueryHandler(
            IWaitingRoomService waitingRoomService,
            IGameService gameService,
            IUserSessionRepository userSessionRepository)
        {
            _waitingRoomService = waitingRoomService;
            _gameService = gameService;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<ActualPlayerStateDto> Handle(GetActualPlayerStateQuery request, CancellationToken cancellationToken)
        {
            var isLoggedIn = _userSessionRepository.IsUserConnected(request.UserId);

            var waitingRoomId = await _waitingRoomService.GetRoomByPlayerIdAsync((int)request.UserId,cancellationToken);
            var gameId = await _gameService.GetGameIdByUserIdAsync(request.UserId);

            return new ActualPlayerStateDto
            {
                IsLoggedIn = isLoggedIn,
                IsInWaitingRoom = waitingRoomId?.GameId,
                IsInGame = gameId?.Id
            };
        }
    }
}
