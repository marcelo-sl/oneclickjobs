using System.ComponentModel.DataAnnotations;

namespace OneClickJobs.Domain.ViewModels.Applications;
public class CreateApplicationViewModel
{
    [Required]
    public int JobId { get; set; }

    [Required(ErrorMessage = "The Resume field is required.")]
    public Guid ResumeId { get; set; } = default!;
}
