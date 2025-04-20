using MediatR;
using SupremeCourt.Domain.Interfaces;


namespace SupremeCourt.Application.CQRS.Auth.Commands
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return false;

            if (!string.IsNullOrWhiteSpace(request.Nickname))
                user.Username = request.Nickname;

            if (request.ProfileImage != null)
            {
                using var ms = new MemoryStream();
                await request.ProfileImage.CopyToAsync(ms);
                user.ProfilePicture = ms.ToArray();
                user.ProfilePictureMimeType = request.ProfileImage.ContentType;
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
