namespace SocialMedia.Application;

public record ExchangeTenantCommand(Guid UserId, string NewTenantId) : ICommand<AuthResponse>;
