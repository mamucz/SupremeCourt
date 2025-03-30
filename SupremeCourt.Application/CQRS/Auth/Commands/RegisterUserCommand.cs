using MediatR;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class RegisterUserCommand : IRequest<bool>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}