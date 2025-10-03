using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace AiPlayers
{
    public class Random : IAiPlayer
    {
        public int Id { get; }
        public string Username { get => "Random"; }
        public string? ProfileImageUrlPath { get; }
        public bool IsAi { get => true; }
        public int Score { get; set; }
        public bool IsEliminated { get ; set; }
        public int numberOfLives { get; set; }
    }
}
