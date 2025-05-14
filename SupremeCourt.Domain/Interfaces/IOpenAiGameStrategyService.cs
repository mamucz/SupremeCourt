using SupremeCourt.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IOpenAiGameStrategyService
    {
        Task<int> DecideMoveAsync(GameRound currentRound, List<GameRound> allRounds, CancellationToken cancellationToken);
    }
}