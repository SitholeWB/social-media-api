namespace SocialMedia.Application.Common.Validation;

public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
