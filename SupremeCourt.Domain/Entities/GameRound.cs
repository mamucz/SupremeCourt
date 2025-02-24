using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupremeCourt.Domain.Entities
{
    public class GameRound
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;

        public int RoundNumber { get; set; }

        [NotMapped] // EF Core nebude mapovat tuto vlastnost
        public Dictionary<int, int> PlayerChoices { get; set; } = new();

        public int CalculatedAverage { get; set; }
        public int WinningPlayerId { get; set; }
    }
}