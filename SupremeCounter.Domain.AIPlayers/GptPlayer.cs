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
        public Guid Id { get; }
        public string Username { get => "GptPlayer"; }
        public string? ProfileImageUrlPath { get; }
        public bool IsAi { get => true; }
        public Guid ActiveWaitingRoom { get; set; }
        public Guid ActiveGame { get; set; }
        public int Score { get; set; }
        public bool IsEliminated { get; set; }
        public int numberOfLives { get ; set; }
    }
}
