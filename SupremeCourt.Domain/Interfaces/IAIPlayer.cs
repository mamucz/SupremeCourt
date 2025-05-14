using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IAiPlayer : IPlayer
    {
        Task<int> MakeChoiceAsync(GameRound currentRound, List<GameRound> allRounds, CancellationToken cancellationToken);
    }
}
