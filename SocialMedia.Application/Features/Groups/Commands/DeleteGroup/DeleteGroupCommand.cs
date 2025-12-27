namespace SocialMedia.Application;

public record DeleteGroupCommand(Guid GroupId) : ICommand<bool>;