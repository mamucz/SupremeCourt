using MediatR;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public string Username { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}