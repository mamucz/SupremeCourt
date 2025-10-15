using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace AiPlayers
{
    public class Random : IPlayer
    {
        public Guid Id { get; }
        public string Username { get; }
        public string? ProfileImageUrlPath { get; }
        public int NumberOfLives { get; set; }
        public bool IsEliminated { get; set; }
        public bool IsAi { get; }
        public Guid ActiveWaitingRoom { get; set; }
        public Guid ActiveGame { get; set; }
    }
}
