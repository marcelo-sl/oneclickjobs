namespace OneClickJobs.Domain.Entities;

/// <summary>
/// Entity base abstract class.
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class EntityBase<TId> where TId : struct
{
    /// <summary>
    /// Gets or sets entity identifier.
    /// </summary>
    public TId Id { get; set; }

    /// <summary>
    /// Gets or sets created at.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets created by.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets updated at.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets updated by.
    /// </summary>
    public Guid? UpdatedBy { get; set; }
}
