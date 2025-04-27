using MediatR;
using SupremeCourt.Application.Common.Interfaces;
using SupremeCourt.Application.CQRS.WaitingRooms.Queries;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Handlers
{
    public class GetActualPlayerStateQueryHandler : IRequestHandler<GetActualPlayerStateQuery, ActualPlayerStateDto>
    {
        private readonly IWaitingRoomSessionManager _waitingRoomSessionManager;
        private readonly IGameService _gameService;
        private readonly IUserSessionRepository _userSessionRepository;

        public GetActualPlayerStateQueryHandler(
            IWaitingRoomSessionManager waitingRoomSessionManager,
            IGameService gameService,
            IUserSessionRepository userSessionRepository)
        {
            _waitingRoomSessionManager = waitingRoomSessionManager;
            _gameService = gameService;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<ActualPlayerStateDto> Handle(GetActualPlayerStateQuery request, CancellationToken cancellationToken)
        {
            var isLoggedIn = _userSessionRepository.IsUserConnected(request.UserId);

            var waitingRoom = _waitingRoomSessionManager.GetSessionByPlayerId((int)request.UserId);
            var game = await _gameService.GetGameIdByUserIdAsync(request.UserId);

            return new ActualPlayerStateDto
            {
                IsLoggedIn = isLoggedIn,
                IsInWaitingRoom = waitingRoom?.WaitingRoomId,
                IsInGame = game?.Id
            };
        }
    }
}