using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IPlayer
    {
        int Id { get; }
        string Username { get; }
        string? ProfileImageUrlPath { get; }
        bool IsAi { get; }
        int Score { get; set; }
        bool IsEliminated { get; set; }

    }
}
