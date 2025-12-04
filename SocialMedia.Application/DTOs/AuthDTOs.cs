namespace SocialMedia.Application;

public record LoginRequest(string Username, string Password);
public record GoogleLoginRequest(string IdToken);
public record RegisterRequest(string Username, string Email, string Password);
public record AuthResponse(string Id, string Username, string Email, string Token);
