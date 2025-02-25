using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SupremeCourt.Application.Services;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace SupremeCourt.Application.Tests
{
    [TestFixture]
    public class GameServiceTests
    {
        private Mock<IGameRepository> _mockGameRepo;
        private Mock<IPlayerRepository> _mockPlayerRepo;
        private GameService _gameService;

        [SetUp]
        public void Setup()
        {
            _mockGameRepo = new Mock<IGameRepository>();
            _mockPlayerRepo = new Mock<IPlayerRepository>();

            _gameService = new GameService(_mockGameRepo.Object, _mockPlayerRepo.Object, new NullLogger<GameService>());
        }

        [Test]
        public async Task CreateGameAsync_ShouldCreateNewGame()
        {
            // Arrange
            _mockGameRepo.Setup(repo => repo.AddAsync(It.IsAny<Game>()))
                .Returns(Task.CompletedTask); // Simulujeme úspěšné vytvoření hry

            // Act
            var game = await _gameService.CreateGameAsync();

            // Assert
            Assert.That(game, Is.Not.Null); // Očekáváme, že metoda vrátí novou hru
            Assert.That(game.IsActive, Is.True); // Hra by měla být aktivní
            _mockGameRepo.Verify(repo => repo.AddAsync(It.IsAny<Game>()), Times.Once); // Ověříme, že byla hra přidána
        }
    }


}