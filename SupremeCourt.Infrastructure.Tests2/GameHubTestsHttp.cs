using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;

namespace SupremeCourt.Infrastructure.Tests2
{
    [TestFixture]
    public class GameHubTestsHttp
    {
        private TestApplicationFactory _factory;
        private HttpClient _httpClient;
        private HubConnection _hubConnection;

        [SetUp]
        public async Task Setup()
        {
            _factory = new TestApplicationFactory();
            _httpClient = _factory.CreateClient();

            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_httpClient.BaseAddress}gameHub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                })
                .Build();

            await _hubConnection.StartAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
            _httpClient.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task GameHub_ShouldReceiveGameUpdate()
        {
            // Arrange
            string receivedMessage = null;
            _hubConnection.On<string>("GameUpdated", message => receivedMessage = message);

            // Act
            await _hubConnection.InvokeAsync("SendGameUpdate", "123", "Test Game Update");

            // Wait for SignalR message to be received
            await Task.Delay(500);

            // Assert
            Assert.That(receivedMessage, Is.EqualTo("Test Game Update"));
        }
    }
}