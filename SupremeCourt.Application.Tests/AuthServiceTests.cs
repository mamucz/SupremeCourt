using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Application.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<IPlayerRepository> _mockPlayerRepo;
        private Mock<ILogger<AuthService>> _mockLogger;
        private AuthService _authService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockPlayerRepo = new Mock<IPlayerRepository>(); // ✅ Přidáno
            _mockLogger = new Mock<ILogger<AuthService>>(); // ✅ Přidáno

            var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var mockTokenBlacklistService = new Mock<TokenBlacklistService>();

            _authService = new AuthService(
                _mockUserRepo.Object,
                _mockPlayerRepo.Object, // ✅ Přidáno
                mockConfig.Object,
                mockTokenBlacklistService.Object,
                _mockLogger.Object // ✅ Přidáno
            );
        }

        [Test]
        public async Task RegisterAsync_ShouldCreateUser_WhenUsernameIsAvailable()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetByUsernameAsync("testuser"))
                        .ReturnsAsync((User)null); // Uživatelské jméno je dostupné

            _mockUserRepo.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                        .Returns(Task.CompletedTask); // Přidání uživatele proběhne úspěšně

            // Act
            var result = await _authService.RegisterAsync("testuser", "password");

            // Assert
            Assert.That(result, Is.True); // Očekáváme úspěch registrace
            _mockUserRepo.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once); // Ověříme, že se zavolalo AddAsync()
        }
    }
}
