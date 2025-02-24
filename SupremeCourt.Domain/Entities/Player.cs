using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupremeCourt.Domain.Entities
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } // Propojení s User tabulkou
       public int Score { get; set; } = 0;
        public bool IsEliminated { get; set; } = false;

        // Navigační vlastnost
        public User User { get; set; } = null!;
    }
}