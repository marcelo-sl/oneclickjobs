using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using OneClickJobs.Domain.Attributes;

namespace OneClickJobs.Domain.ViewModels.Resumes;

public class CreateResumeViewModel
{
    [Required]
    [FileExtension([".pdf"])]
    public IFormFile FormFile { get; set; } = null!;
}
