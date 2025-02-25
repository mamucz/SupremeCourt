using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.Tests2
{
    [TestFixture]
    public class GameHubTests : IDisposable
    {
        private GameHub _gameHub;
        private Mock<IHubCallerClients> _mockClients;
        private Mock<IClientProxy> _mockClientProxy;
        private Mock<IGroupManager> _mockGroups;
        private bool _disposed;

        [SetUp]
        public void Setup()
        {
            _mockClients = new Mock<IHubCallerClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockGroups = new Mock<IGroupManager>();

            _mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
            _gameHub = new GameHub
            {
                Clients = _mockClients.Object,
                Groups = _mockGroups.Object
            };
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }
        

        [Test]
        public async Task SendGameUpdate_ShouldCallGameUpdated_OnGroupClients()
        {
            var gameId = "123";
            var message = "Game updated!";

            await _gameHub.SendGameUpdate(gameId, message);

            _mockClientProxy.Verify(client => client.SendCoreAsync("GameUpdated", new object[] { message }, default), Times.Once);
        }

        [Test]
        public async Task JoinGame_ShouldAddClientToGroup()
        {
            var gameId = "123";
            var connectionId = "test-connection-id";

            var mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(ctx => ctx.ConnectionId).Returns(connectionId);
            _gameHub.Context = mockContext.Object;

            await _gameHub.JoinGame(gameId);

            _mockGroups.Verify(groups => groups.AddToGroupAsync(connectionId, gameId, default), Times.Once);
        }
        public void Dispose()
        {
            if (!_disposed)
            {
                _gameHub = null;
                _disposed = true;
            }
        }
    }
}
