namespace OneClickJobs.Domain.ViewModels.Resumes;

public class ViewResumeViewModel(Guid id, string fileName, byte[] fileContent, DateTimeOffset createdAt)
{
    public Guid Id { get; set; } = id;
    public string FileName { get; set; } = fileName;
    public DateTimeOffset CreatedAt { get; set; } = createdAt;

    public string GetFileBase64String()
    {
        return "data:application/pdf;base64," + Convert.ToBase64String(fileContent);
    }
    
}
