namespace SupremeCourt.Domain.Entities
{
    public record Player
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
        public bool IsEliminated { get; set; } = false;
    }
}