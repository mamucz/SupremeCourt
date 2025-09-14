using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class AddAiPlayerToRoomCommandHandler : IRequestHandler<AddAiPlayerToRoomCommand, bool>
    {
        private readonly IWaitingRoomSessionManager _sessionManager;
        private readonly IAiPlayerFactory _aiPlayerFactory;
        private readonly IPlayerRepository _playerRepository;

        public AddAiPlayerToRoomCommandHandler(
            IWaitingRoomSessionManager sessionManager,
            IAiPlayerFactory aiPlayerFactory,
            IPlayerRepository playerRepository)
        {
            _sessionManager = sessionManager;
            _aiPlayerFactory = aiPlayerFactory;
            _playerRepository = playerRepository;
        }

        public async Task<bool> Handle(AddAiPlayerToRoomCommand request, CancellationToken cancellationToken)
        {
            await _playerRepository.EnsureAiPlayerExistsAsync(request.Type, cancellationToken);
            var aiPlayer = await _aiPlayerFactory.CreateAsync(request.Type);

            return await _sessionManager.TryAddPlayerToRoomAsync(request.WaitingRoomId, aiPlayer, cancellationToken);
        }
    }

}
