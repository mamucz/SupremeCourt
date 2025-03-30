using MediatR;
using SupremeCourt.Application.Services;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, bool>
    {
        private readonly TokenBlacklistService _tokenBlacklistService;

        public LogoutUserCommandHandler(TokenBlacklistService tokenBlacklistService)
        {
            _tokenBlacklistService = tokenBlacklistService;
        }

        public Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Token))
                return Task.FromResult(false);

            _tokenBlacklistService.BlacklistToken(request.Token);
            return Task.FromResult(true);
        }
    }
}