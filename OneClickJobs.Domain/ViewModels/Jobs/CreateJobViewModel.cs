using System.ComponentModel.DataAnnotations;

namespace OneClickJobs.Domain.ViewModels.Jobs;

public class CreateJobViewModel
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;
}
