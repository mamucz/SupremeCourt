using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SupremeCourt.Infrastructure.Services
{
    public class OpenAiGameStrategyService : IOpenAiGameStrategyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string Model = "gpt-4";

        public OpenAiGameStrategyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:Key"];
        }

        public async Task<int> DecideMoveAsync(GameRound currentRound, List<GameRound> allRounds, CancellationToken cancellationToken)
        {
            var prompt = BuildPrompt(allRounds, currentRound.RoundNumber);

            var request = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "system", content = "Jsi AI hráč logické hry. V každém kole si vybíráš číslo 0–100, abys byl co nejblíž průměru ostatních hráčů." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var parsed = JsonDocument.Parse(json);
            var resultText = parsed.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (int.TryParse(ExtractFirstNumber(resultText), out var result) && result >= 0 && result <= 100)
                return result;

            return new Random().Next(0, 101); // fallback
        }

        private string BuildPrompt(List<GameRound> rounds, int currentRound)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Toto je kolo č. {currentRound}. Zde je historie:");
            foreach (var round in rounds)
            {
                sb.AppendLine($"Kolo {round.RoundNumber}:");
                foreach (var choice in round.PlayerChoices)
                    sb.AppendLine($" - Hráč {choice.Key}: {choice.Value}");
                sb.AppendLine($" -> Průměr: {round.CalculatedAverage}, vítěz: {round.WinningPlayerId}");
                sb.AppendLine();
            }

            sb.AppendLine("Zvol číslo mezi 0 a 100, které je co nejblíž průměru ostatních. Odpověz pouze číslem.");
            return sb.ToString();
        }

        private string ExtractFirstNumber(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    sb.Append(c);
                else if (sb.Length > 0)
                    break;
            }

            return sb.ToString();
        }
    }
}
