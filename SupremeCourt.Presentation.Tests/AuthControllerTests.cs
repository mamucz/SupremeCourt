using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using SupremeCourt.Presentation.Controllers;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Presentation.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService; // ✅ Použijeme interface místo konkrétní třídy
        private Mock<TokenBlacklistService> _mockTokenBlacklistService;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>(); // ✅ Opraveno
            _mockTokenBlacklistService = new Mock<TokenBlacklistService>();

            _authController = new AuthController(
                _mockAuthService.Object,
                _mockTokenBlacklistService.Object
            );
        }

        [Test]
        public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var userDto = new UserDto("testuser", "password");
            _mockAuthService.Setup(service => service.RegisterAsync(userDto.Username, userDto.Password))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.Register(userDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo("Registration successful"));
        }
    }
}