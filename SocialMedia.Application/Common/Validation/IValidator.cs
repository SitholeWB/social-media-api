namespace SocialMedia.Application;

public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T instance, CancellationToken cancellationToken = default);
}