namespace OneClickJobs.Web.Data.ViewModels.Resumes;

public class ViewResumeViewModel
{
    public Guid Id { get; set; }
    public required string FileName { get; set; }
    public required string Base64string { get; set; }
}
