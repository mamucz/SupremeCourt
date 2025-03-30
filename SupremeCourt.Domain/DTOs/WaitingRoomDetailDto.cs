using SupremeCourt.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCourt.Domain.DTOs
{
    public class WaitingRoomDetailDto
    {
        public int WaitingRoomId { get; set; }
        public List<string> Players { get; set; } = new();
        public string CreatedBy { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int PlayerCount => Players.Count;
        public bool CanStartGame => Players.Count >= GameRules.MaxPlayers;
        public int TimeLeftSeconds { get; set; }
    }

}
