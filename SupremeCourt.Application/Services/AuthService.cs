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
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly TokenBlacklistService _tokenBlacklistService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, TokenBlacklistService tokenBlacklistService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenBlacklistService = tokenBlacklistService;
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
            await _userRepository.AddAsync(user);

            _logger.LogInformation("Uživatel {Username} úspěšně zaregistrován", username);
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
            _logger.LogInformation("Smazání uživatele: {Username}", username);

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Uživatel {Username} nebyl nalezen", username);
                return false;
            }

            await _userRepository.DeleteAsync(user);
            _tokenBlacklistService.BlacklistToken(token);

            _logger.LogInformation("Uživatel {Username} úspěšně smazán", username);
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
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
