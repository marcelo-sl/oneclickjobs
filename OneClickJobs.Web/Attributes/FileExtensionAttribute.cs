using System.ComponentModel.DataAnnotations;

namespace OneClickJobs.Web.Attributes;

public class FileExtensionAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;

    public FileExtensionAttribute(string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions;
    }

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    {
        if (value != null)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult("Only files with extension " + string.Join(", ", _allowedExtensions) + " are allowed.");
                }
            }
        }

        return ValidationResult.Success!;
    }
}
