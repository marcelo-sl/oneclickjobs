namespace OneClickJobs.Domain.Entities;

/// <summary>
/// Application class.
/// </summary>
public sealed class Application : EntityBase<Guid>
{
    /// <summary>
    /// Gets or sets job.
    /// </summary>
    public Job Job { get; set; } = default!;

    /// <summary>
    /// Gets or sets resume.
    /// </summary>
    public Resume Resume { get; set; } = default!;
}
