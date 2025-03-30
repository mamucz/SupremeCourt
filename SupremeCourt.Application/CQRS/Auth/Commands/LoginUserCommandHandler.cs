using MediatR;
using SupremeCourt.Application.CQRS.Auth.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResultDto?>
    {
        private readonly IAuthService _authService;

        public LoginUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginResultDto?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var token = await _authService.AuthenticateAsync(request.Username, request.Password);
            var user = await _authService.GetUserByUsernameAsync(request.Username);

            if (token == null || user == null)
                return null;

            return new LoginResultDto
            {
                Token = token,
                UserId = user.Id
            };
        }
    }
}