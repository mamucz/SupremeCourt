using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupremeCourt.Presentation.Controllers;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Presentation.Tests
{
    [TestFixture]
    public class GameControllerTests
    {
        private Mock<ICreateGameHandler> _mockCreateGameHandler;
        private Mock<IGameService> _mockGameService;
        private Mock<IWaitingRoomListService> _mockWaitingRoomService;
        private Mock<ILogger<GameController>> _mockLogger;
        private GameController _gameController;

        [SetUp]
        public void Setup()
        {
            _mockCreateGameHandler = new Mock<ICreateGameHandler>();
            _mockGameService = new Mock<IGameService>();
            _mockWaitingRoomService = new Mock<IWaitingRoomListService>();
            _mockLogger = new Mock<ILogger<GameController>>();

            _gameController = new GameController(
                _mockCreateGameHandler.Object,
                _mockGameService.Object,
                _mockWaitingRoomService.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task JoinGame_ShouldReturnOk_WhenPlayerJoinsSuccessfully()
        {
            // Arrange
            var joinRequest = new JoinGameRequest { GameId = 1, PlayerId = 100 };

            _mockWaitingRoomService.Setup(service => service.JoinWaitingRoomAsync(joinRequest.GameId, joinRequest.PlayerId))
                                   .ReturnsAsync(true);

            // Act
            var result = await _gameController.JoinGame(joinRequest) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo("Joined successfully."));
        }

        [Test]
        public async Task JoinGame_ShouldReturnBadRequest_WhenJoinFails()
        {
            // Arrange
            var joinRequest = new JoinGameRequest { GameId = 1, PlayerId = 100 };

            _mockWaitingRoomService.Setup(service => service.JoinWaitingRoomAsync(joinRequest.GameId, joinRequest.PlayerId))
                                   .ReturnsAsync(false); // Simulujeme selhání

            // Act
            var result = await _gameController.JoinGame(joinRequest) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Is.EqualTo("Failed to join the game."));
        }
    }
}
