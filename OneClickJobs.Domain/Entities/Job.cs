namespace OneClickJobs.Domain.Entities;

public sealed class Job : EntityBase<int>
{
    /// <summary>
    /// Gets or sets title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets description.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets status.
    /// </summary>
    public JobStatus Status { get; private set; }

    /// <summary>
    /// Tries to change job status to <see cref="JobStatus.Closed"/>.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>A <see cref="bool"/> value, true if changed successfully.
    /// </returns>
    public bool TryClose(Guid userId)
    {
        if (userId != CreatedBy)
            return false;

        Status = JobStatus.Closed;
        return true;
    }
}
