using MediatR;
using SupremeCourt.Application.CQRS.Auth.DTOs;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class LoginUserCommand : IRequest<LoginResultDto>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}