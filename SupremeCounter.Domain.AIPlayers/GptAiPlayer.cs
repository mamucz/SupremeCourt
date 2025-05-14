using SupremeCourt.Domain.AIPlayers;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.AIPlayers
{
    public class GptAiPlayer : IAiPlayer
    {
        public int Id { get; private set; }
        public string Username => "GPT Bot";
        public string? ProfileImageUrlPath => null;
        public bool IsAi => true;
        public int Score { get; set; }
        public bool IsEliminated { get; set; }

        private readonly IOpenAiGameStrategyService _strategyService;

        public GptAiPlayer(int id, IOpenAiGameStrategyService strategyService)
        {
            Id = id;
            _strategyService = strategyService;
        }

        public Task<int> MakeChoiceAsync(GameRound currentRound, List<GameRound> allRounds, CancellationToken cancellationToken)
        {
            return _strategyService.DecideMoveAsync(currentRound, allRounds, cancellationToken);
        }

    }
}
