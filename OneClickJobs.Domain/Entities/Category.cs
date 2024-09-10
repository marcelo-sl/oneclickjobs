namespace OneClickJobs.Domain.Entities;

/// <summary>
/// Category entity.
/// </summary>
public class Category : EntityBase<int>
{
    /// <summary>
    /// Gets or sets name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets jobs.
    /// </summary>
    public List<Job> Jobs { get; set; } = [];
}
