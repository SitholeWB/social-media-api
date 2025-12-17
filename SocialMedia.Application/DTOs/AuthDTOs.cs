namespace SocialMedia.Application;

public record LoginRequest(string Username, string Password);
public record GoogleLoginRequest(string IdToken);
public record RegisterRequest(string Username, string Email, string Password)
{
    public string Names { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}
public record AuthResponse(string Id, string Username, string Email, string Names, string Surname, string Token);