namespace SupremeCourt.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
    public int PlayerId { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => !IsRevoked && !IsExpired;
}
