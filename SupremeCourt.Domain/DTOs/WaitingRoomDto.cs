using SupremeCourt.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.DTOs
{
    public class WaitingRoomDto
    {
        public Guid WaitingRoomId { get; set; } 
        public List<PlayerDto> Players { get; set; } = new();
        public Guid CreatedByPlayerId { get; set; }
        public string CreatedByPlayerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PlayerCount => Players.Count;
        public bool CanStartGame => Players.Count >= GameRules.MaxPlayers;
        public int TimeLeftSeconds { get; set; }

    }

}
