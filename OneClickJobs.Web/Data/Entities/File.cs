namespace OneClickJobs.Web.Data.Entities;

/// <summary>
/// File abstract class.
/// </summary>
public abstract class File : EntityBase<Guid>
{
    /// <summary>
    /// Gets or sets file name.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Gets or sets file content.
    /// </summary>
    public byte[] FileContent { get; set; } = [];
}
