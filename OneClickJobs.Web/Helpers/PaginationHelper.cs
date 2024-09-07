namespace OneClickJobs.Web.Helpers;

public static class PaginationHelper
{
    private const int RecordLimit = 200;

    public static int SkipRecords(int page, int size) => (page - 1) * size;
    public static int TakeRecords(int size) => size < RecordLimit ? size : RecordLimit;
}
