namespace OneClickJobs.Web.Helpers;

public class FileHelper
{
    public static async Task<byte[]> ConvertToArrayAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
}
