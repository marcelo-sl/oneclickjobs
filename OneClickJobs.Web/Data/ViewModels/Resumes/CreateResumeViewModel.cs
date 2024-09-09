using System.ComponentModel.DataAnnotations;

using OneClickJobs.Web.Attributes;

namespace OneClickJobs.Web.Data.ViewModels.Resumes;

public class CreateResumeViewModel
{
    [Required]
    [FileExtensionAttribute([".pdf"])]
    public IFormFile FormFile { get; set; } = null!;
}
