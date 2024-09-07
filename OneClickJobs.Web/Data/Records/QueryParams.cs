namespace OneClickJobs.Web.Data.Records;

public record QueryParams
{
    public QueryParams()
    {
        Page = 1;
        Size = 10;
    }

    public string? Search { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
}
