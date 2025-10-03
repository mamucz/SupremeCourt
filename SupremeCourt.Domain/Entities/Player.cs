using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Domain.Entities
{
    public class Player : IPlayer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } // Propojení s User tabulkou

        public int numberOfLives { get; set; }
        public bool IsEliminated { get; set; } = false;
        // Navigační vlastnost
        public User User { get; set; } = null!;
        public string Username => User.Username;
        public string? ProfileImageUrlPath => User.ProfileImageUrlPath;

        public bool IsAi => User.IsAi;

    }
}