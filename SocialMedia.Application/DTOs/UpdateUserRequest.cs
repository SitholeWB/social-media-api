namespace SocialMedia.Application;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}