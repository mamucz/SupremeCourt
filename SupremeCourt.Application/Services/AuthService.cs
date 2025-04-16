using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly TokenBlacklistService _tokenBlacklistService;
        private readonly ILogger<AuthService> _logger;
        private readonly IPlayerRepository _playerRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(IUserRepository userRepository, IPlayerRepository playerRepository, IConfiguration configuration, TokenBlacklistService tokenBlacklistService, IRefreshTokenRepository refreshTokenRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _configuration = configuration;
            _tokenBlacklistService = tokenBlacklistService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;

        }

        /// <summary>
        /// Přihlášení uživatele a generování JWT tokenu.
        /// </summary>
        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            _logger.LogInformation("Pokus o přihlášení uživatele: {Username}", username);

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Neplatné přihlašovací údaje pro uživatele: {Username}", username);
                return null;
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation("Uživatel {Username} úspěšně přihlášen", username);
            return token;
        }

        /// <summary>
        /// Registrace nového uživatele.
        /// </summary>
        public async Task<bool> RegisterAsync(string username, string password)
        {
            _logger.LogInformation("Registrace uživatele: {Username}", username);

            if (await _userRepository.GetByUsernameAsync(username) != null)
            {
                _logger.LogWarning("Uživatel {Username} již existuje", username);
                return false;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Username = username, PasswordHash = hashedPassword };

            // Vytvoříme i Player při registraci Usera
            var player = new Player { User = user, UserId = user.Id };

            await _userRepository.AddAsync(user);
            await _playerRepository.AddAsync(player); // Nová logika

            _logger.LogInformation("Uživatel {Username} a jeho Player účet úspěšně vytvořeni", username);
            return true;
        }


        /// <summary>
        /// Odhlášení uživatele (přidání tokenu na blacklist).
        /// </summary>
        public void LogoutUser(string token)
        {
            _logger.LogInformation("Odhlášení uživatele s tokenem: {Token}", token);
            _tokenBlacklistService.BlacklistToken(token);
        }

        /// <summary>
        /// Smazání uživatele a zneplatnění jeho tokenu.
        /// </summary>
        public async Task<bool> DeleteUserAsync(string username, string token)
        {
            _logger.LogInformation("Mazání uživatele: {Username}", username);

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.Deleted)
            {
                _logger.LogWarning("Uživatel {Username} nenalezen nebo již smazán", username);
                return false;
            }

            user.Deleted = true; // Pouze označíme uživatele jako smazaného
            await _userRepository.UpdateAsync(user);
            _tokenBlacklistService.BlacklistToken(token);

            _logger.LogInformation("Uživatel {Username} úspěšně označen jako smazaný", username);
            return true;
        }




        /// <summary>
        /// Generování JWT tokenu pro uživatele.
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(100),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }
        public async Task<RefreshToken> GenerateRefreshTokenAsync(int playerId)
        {
            return await _refreshTokenRepository.CreateAsync(playerId);
        }

        string IAuthService.GenerateJwtToken(User user)
        {
            return GenerateJwtToken(user);
        }

    }
}
