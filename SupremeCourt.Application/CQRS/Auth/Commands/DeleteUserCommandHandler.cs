using MediatR;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IAuthService _authService;

        public DeleteUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _authService.DeleteUserAsync(request.Username, request.Token);
        }
    }
}