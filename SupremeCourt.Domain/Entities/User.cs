namespace SupremeCourt.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public bool Deleted { get; set; } = false; // Nový atribut místo mazání
        public Player? Player { get; set; } // Navigační vlastnost
        public byte[]? ProfilePicture { get; set; }
        public string? ProfilePictureMimeType { get; set; } // např. "image/png"

        //stats
        public int WinGames { get; set; } = 0;
        public int LostGames { get; set; } = 0;

        public int TimeForRound { get; set; } = 0;
        // AI
        public bool IsAi { get; set; } = false;
        public string TypeName { get; set; }

        public string? ProfileImageUrlPath => $"/api/auth/{Id}/profile-picture";

    }
}