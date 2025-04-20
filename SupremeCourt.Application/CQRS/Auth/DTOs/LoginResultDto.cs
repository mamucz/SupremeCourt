namespace SupremeCourt.Application.CQRS.Auth.DTOs
{
    public class LoginResultDto
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}