using MediatR;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class AddAiPlayersBulkCommandHandler : IRequestHandler<AddAiPlayersBulkCommand, Unit>
    {
        private readonly IWaitingRoomSessionManager _sessionManager;

        public AddAiPlayersBulkCommandHandler(IWaitingRoomSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public async Task<Unit> Handle(AddAiPlayersBulkCommand request, CancellationToken cancellationToken)
        {
            // validace vstupů
            if (request.Count <= 0)
                return Unit.Value;

            // existuje roomka?
            var session = _sessionManager.GetSession(request.WaitingRoomId);
            if (session is null)
                throw new InvalidOperationException("Waiting room not found.");

            for (int i = 0; i < request.Count; i++)
            {
                // voláme manager, který vytvoří AI hráče ve scoped DI
                var ok = await _sessionManager.AddAiPlayerAsync(request.WaitingRoomId, request.Type, cancellationToken);
                if (!ok)
                    break; // room může být plná apod. (volitelné: vyhoď doménovou chybu)
            }

            return Unit.Value;
        }
    }
}