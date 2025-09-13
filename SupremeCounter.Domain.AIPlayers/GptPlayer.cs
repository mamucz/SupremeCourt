using SupremeCourt.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiPlayers
{
    public class GptPlayer : IAiPlayer
    {
        public int Id { get; }
        public string Username { get => "GptPlayer"; }
        public string? ProfileImageUrlPath { get; }
        public bool IsAi { get => true; }
        public int Score { get; set; }
        public bool IsEliminated { get; set; }
    }
}
