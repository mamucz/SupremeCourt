using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.Interfaces
{
    public interface ITokenBlacklistService
    {
        void BlacklistToken(string token);
        bool IsTokenBlacklisted(string token);
    }
}
