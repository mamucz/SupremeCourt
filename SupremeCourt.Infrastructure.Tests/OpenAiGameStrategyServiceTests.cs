using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Moq.Protected;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Infrastructure.Services;

namespace SupremeCourt.Infrastructure.Tests
{
    public class OpenAiGameStrategyServiceTests
    {
        [Test]
        public async Task DecideMoveAsync_Returns_ValidNumber_FromOpenAi()
        {
            // Arrange
            var fakeJsonResponse = """
            {
              "choices": [
                {
                  "message": {
                    "content": "42"
                  }
                }
              ]
            }
            """;

            var mockHandler = new Mock<HttpMessageHandler>();

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeJsonResponse),
                });

            var httpClient = new HttpClient(mockHandler.Object);

            var service = new OpenAiGameStrategyService(httpClient, "fake-api-key");

            var currentRound = new GameRound
            {
                RoundNumber = 3,
                PlayerChoices = new() { { 1, 33 }, { 2, 77 } }
            };

            var history = new List<GameRound>
            {
                new GameRound
                {
                    RoundNumber = 1,
                    PlayerChoices = new() { { 1, 20 }, { 2, 40 } },
                    CalculatedAverage = 30,
                    WinningPlayerId = 1
                },
                new GameRound
                {
                    RoundNumber = 2,
                    PlayerChoices = new() { { 1, 50 }, { 2, 60 } },
                    CalculatedAverage = 55,
                    WinningPlayerId = 2
                }
            };

            // Act
            var result = await service.DecideMoveAsync(currentRound, history, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(42));
        }
    }
}
