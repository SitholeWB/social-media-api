namespace SocialMedia.Domain;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Names { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsBanned { get; set; }
    public DateTimeOffset? LastActiveAt { get; set; }

    public string GetFullName()
    {
        if (string.IsNullOrWhiteSpace(Names) && string.IsNullOrWhiteSpace(Surname))
        {
            return Username;
        }
        if (string.IsNullOrWhiteSpace(Names))
        {
            return Surname;
        }
        if (string.IsNullOrWhiteSpace(Surname))
        {
            return Names;
        }
        return $"{Names} {Surname}".Trim();
    }
}