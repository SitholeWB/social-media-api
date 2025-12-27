namespace SocialMedia.Application;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = new List<ValidationError>();

    public void AddError(string propertyName, string errorMessage)
    {
        Errors.Add(new ValidationError(propertyName, errorMessage));
    }
}