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

    }
}