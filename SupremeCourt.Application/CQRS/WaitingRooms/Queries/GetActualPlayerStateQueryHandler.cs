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
            var isLoggedIn = false; //TODO: await _userSessionRepository.IsUserLoggedInAsync(request.PlayerId);

            var waitingRoom = _waitingRoomSessionManager.GetSessionByPlayerId(request.PlayerId);
           
            return new ActualPlayerStateDto
            {
                IsLoggedIn = isLoggedIn,
                IsInWaitingRoom = waitingRoom?.WaitingRoomId,
                IsInGame = 0
            };
        }
    }
}