using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Domain.Entities
{
    /// <summary>
    /// Hráč během hry nebo v místnosti – runtime objekt odvozený od User.
    /// Neuchovává se v databázi.
    /// </summary>
    public class Player : IPlayer
    {
        public Guid Id { get; set; } // Jedinečný ID hráče v runtime (nebo klidně přebrat z User.Id)

        public int NumberOfLives { get; set; } = 3;
        public bool IsEliminated { get; set; } = false;

        public Guid ActiveWaitingRoom { get; set; } = Guid.Empty;
        public Guid ActiveGame { get; set; } = Guid.Empty;

        public User User { get; set; }

        public string Username => User.Username;
        public string? ProfileImageUrlPath => User.ProfileImageUrlPath;
        public bool IsAi => User.IsAi;

        public int UserId => User.Id;

        public Player(User user)
        {
            User = user;
            Id = Guid.NewGuid(); // runtime ID hráče (např. pro mapování ve hrách)
        }
    }
}