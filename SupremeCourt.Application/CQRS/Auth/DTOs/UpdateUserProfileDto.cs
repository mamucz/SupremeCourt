using Microsoft.AspNetCore.Http;

public class UpdateUserProfileDto
{
    public string? Nickname { get; set; }

    public IFormFile? ProfileImage { get; set; }
}