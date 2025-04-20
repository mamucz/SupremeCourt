using MediatR;
using Microsoft.AspNetCore.Http;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class RegisterUserCommand : IRequest<bool>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public IFormFile? ProfilePicture { get; set; }
    }
}
