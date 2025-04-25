using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IAIPlayerDefinition
    {
        string Username { get; }
        string BotType { get; }

        // Může být i avatar
        string? ProfilePicturePath => null;
    }
}
