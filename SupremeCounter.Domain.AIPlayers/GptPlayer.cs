using SupremeCourt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupremeCourt.Domain.Entities;

namespace AiPlayers
{
    public class GptPlayer : Player, IAiPlayer
    {
        public GptPlayer(User user) : base(user)
        {
            // Můžeš zde např. validovat, že user.IsAi == true && user.TypeName == "Gpt"
        }
    }
}
