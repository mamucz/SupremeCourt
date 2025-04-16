using MediatR;
using SupremeCourt.Application.CQRS.Auth.Commands;
using SupremeCourt.Domain.Interfaces;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, string?>
{
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepo,
        IUserRepository userRepository,
        IAuthService authService)
    {
        _refreshTokenRepo = refreshTokenRepo;
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<string?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (token == null || token.Expires < DateTime.UtcNow)
            return null;

        var user = await _userRepository.GetByIdAsync(token.PlayerId);
        if (user == null)
            return null;

        await _refreshTokenRepo.InvalidateAsync(request.RefreshToken, cancellationToken); // Jednorázový token

        return _authService.GenerateJwtToken(user);
    }
}
