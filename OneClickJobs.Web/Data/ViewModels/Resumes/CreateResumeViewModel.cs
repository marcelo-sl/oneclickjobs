using System.ComponentModel.DataAnnotations;

namespace OneClickJobs.Web.Data.ViewModels.Resumes;

public class CreateResumeViewModel
{
    [Required]
    public IFormFile FormFile { get; set; } = null!;
}
