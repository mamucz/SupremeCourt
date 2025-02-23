namespace SupremeCourt.Domain.Entities
{
    public record Game
    {
        public int Id { get; set; }
        public List<Player> Players { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoundNumber { get; set; } = 1;
        public List<GameRound> Rounds { get; set; } = new();
    }
}