using SocialMedia.Application.Common.Validation;

namespace SocialMedia.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommandValidator : IValidator<CreateGroupCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateGroupCommand instance, CancellationToken cancellationToken = default)
    {
        var result = new ValidationResult();

        if (string.IsNullOrEmpty(instance.Name))
        {
            result.AddError(nameof(instance.Name), "Name is required.");
        }
        else if (instance.Name.Length > 100)
        {
            result.AddError(nameof(instance.Name), "Name must not exceed 100 characters.");
        }

        if (!string.IsNullOrEmpty(instance.Description) && instance.Description.Length > 5000)
        {
            result.AddError(nameof(instance.Description), "Description must not exceed 5000 characters.");
        }

        return Task.FromResult(result);
    }
}
