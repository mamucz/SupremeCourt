using MediatR;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IAuthService _authService;

        public RegisterUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            byte[]? imageData = null;
            string? mimeType = null;

            if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                using var ms = new MemoryStream();
                await request.ProfilePicture.CopyToAsync(ms, cancellationToken);
                imageData = ms.ToArray();
                mimeType = request.ProfilePicture.ContentType;
            }
            return await _authService.RegisterAsync(request.Username, request.Password, imageData, mimeType);
        }
    }
}