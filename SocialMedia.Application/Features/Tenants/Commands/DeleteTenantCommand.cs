namespace SocialMedia.Application;

public record DeleteTenantCommand(Guid Id) : ICommand<bool>;
