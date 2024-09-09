namespace OneClickJobs.Domain.ViewModels;

public record QueryParamsViewModel
{
    public QueryParamsViewModel()
    {
        Page = 1;
        Size = 10;
    }

    public string? Search { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
}
