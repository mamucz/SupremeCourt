using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.SignalR;
using SupremeCourt.Infrastructure.SignalR;

namespace SupremeCourt.Infrastructure.Tests2
{
    [TestFixture]
    public class WaitingRoomHubTests : IDisposable
    {
        private WaitingRoomHub _waitingRoomHub;
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
            _waitingRoomHub = new WaitingRoomHub
            {
                Clients = _mockClients.Object,
                Groups = _mockGroups.Object
            };
        }

        [TearDown]
       

        [Test]
        public async Task JoinRoom_ShouldAddClientToGroup_AndNotifyOthers()
        {
            var gameId = "456";
            var connectionId = "test-connection-id";
            var expectedMessage = $"Hráč se připojil k místnosti {gameId}";

            var mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(ctx => ctx.ConnectionId).Returns(connectionId);
            _waitingRoomHub.Context = mockContext.Object;

            await _waitingRoomHub.JoinRoom(gameId);

            _mockGroups.Verify(groups => groups.AddToGroupAsync(connectionId, gameId, default), Times.Once);
            _mockClientProxy.Verify(client => client.SendCoreAsync("PlayerJoined", new object[] { expectedMessage }, default), Times.Once);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _waitingRoomHub = null;
                _disposed = true;
            }
        }
    }
}