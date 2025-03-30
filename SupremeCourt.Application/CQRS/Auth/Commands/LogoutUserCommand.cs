using MediatR;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class LogoutUserCommand : IRequest<bool>
    {
        public string Token { get; set; } = null!;
    }
}