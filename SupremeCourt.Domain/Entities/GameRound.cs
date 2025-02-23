namespace SupremeCourt.Domain.Entities
{
    public record GameRound
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public List<int> SubmittedNumbers { get; set; } = new();
        public double AverageResult { get; set; }
        public int WinningNumber { get; set; }
    }
}