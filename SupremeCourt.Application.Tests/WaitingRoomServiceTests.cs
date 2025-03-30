using System.Collections.Generic;
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
    public class WaitingRoomServiceTests
    {
        private Mock<IWaitingRoomRepository> _mockWaitingRoomRepo;
        private Mock<IGameRepository> _mockGameRepo;
        private Mock<IPlayerRepository> _mockPlayerRepo;
        private Mock<IWaitingRoomListNotifier> _mockNotifier;
        private Mock<IGameService> _mockGameService;
        private Mock<ILogger<WaitingRoomListService>> _mockLogger;

        private WaitingRoomListService _waitingRoomService;

        [SetUp]
        public void Setup()
        {
            _mockWaitingRoomRepo = new Mock<IWaitingRoomRepository>();
            _mockGameRepo = new Mock<IGameRepository>();
            _mockPlayerRepo = new Mock<IPlayerRepository>();
            _mockNotifier = new Mock<IWaitingRoomListNotifier>();
            _mockGameService = new Mock<IGameService>();
            _mockLogger = new Mock<ILogger<WaitingRoomListService>>();

            _waitingRoomService = new WaitingRoomListService(
                _mockWaitingRoomRepo.Object,
                _mockGameRepo.Object,
                _mockPlayerRepo.Object,
                _mockGameService.Object,
                _mockNotifier.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task CreateWaitingRoomAsync_ShouldCreateRoom_WhenGameExists()
        {
            // Arrange: Create a mock game that exists in the repository
            var game = new Game { Id = 1, IsActive = false };
            _mockGameRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(game);
            _mockWaitingRoomRepo.Setup(repo => repo.AddAsync(It.IsAny<WaitingRoom>())).Returns(Task.CompletedTask);

            // Act: Call the method to create a waiting room
            var waitingRoom = await _waitingRoomService.CreateWaitingRoomAsync(1);

            // Assert: Verify that the waiting room was successfully created
            Assert.That(waitingRoom, Is.Not.Null, "Waiting room should be created.");
            Assert.That(waitingRoom.GameId, Is.EqualTo(1), "Waiting room should be linked to the correct game.");
            _mockWaitingRoomRepo.Verify(repo => repo.AddAsync(It.IsAny<WaitingRoom>()), Times.Once, "Waiting room should be added to the repository.");
        }

        [Test]
        public async Task JoinWaitingRoomAsync_ShouldAllowUpToFivePlayers_AndStartGame()
        {
            // Arrange: Create a mock game and an empty waiting room
            var gameId = 1;
            var game = new Game { Id = gameId, IsActive = false };
            var waitingRoom = new WaitingRoom { GameId = gameId, Players = new List<Player>() };

            _mockGameRepo.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync(game);
            _mockWaitingRoomRepo.Setup(repo => repo.GetByGameIdAsync(gameId)).ReturnsAsync(waitingRoom);
            _mockWaitingRoomRepo.Setup(repo => repo.UpdateAsync(It.IsAny<WaitingRoom>())).Returns(Task.CompletedTask);
            _mockGameService.Setup(service => service.StartGameAsync(gameId)).ReturnsAsync(true);
            _mockNotifier.Setup(notifier => notifier.NotifyPlayerJoinedAsync(gameId, It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act: Simulate adding 5 players to the waiting room
            for (int i = 1; i <= 5; i++)
            {
                var player = new Player { Id = i, User = new User { Username = $"Player{i}" } };
                _mockPlayerRepo.Setup(repo => repo.GetByIdAsync(i)).ReturnsAsync(player);

                var result = await _waitingRoomService.JoinWaitingRoomAsync(gameId, i);
                Assert.That(result, Is.True, $"Player {i} should successfully join.");
            }

            // Assert: Ensure that the waiting room has exactly 5 players
            Assert.That(waitingRoom.Players.Count, Is.EqualTo(5), "Waiting room should contain exactly 5 players.");

            // Verify that the game is automatically started when the 5th player joins
            _mockGameService.Verify(service => service.StartGameAsync(gameId), Times.Once, "The game should start automatically after 5 players join.");
        }
    }
}
