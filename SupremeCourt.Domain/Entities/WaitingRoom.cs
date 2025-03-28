using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupremeCourt.Domain.Entities
{
    public class WaitingRoom
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Game")]
        public int? GameId { get; set; }
        public Game? Game { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Player> Players { get; set; } = new();
        public int CreatedByPlayerId { get; set; } // ✅ ID hráče, který založil místnost
    }
}