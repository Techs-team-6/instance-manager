using Microsoft.Extensions.Logging;

namespace InstanceManager.Services;

public class DownloadService
{
    private readonly ILogger<DownloadService> _logger;

    public DownloadService(ILogger<DownloadService> logger)
    {
        _logger = logger;
    }

    public void DownloadFile(string address, FileInfo fileName)
    {
        _logger.LogInformation("Downloading File \"{0}\" from \"{1}\" .......\n", fileName, address);
        var client = new HttpClient();
        var bytes = client.GetByteArrayAsync(address).Result;
        Directory.CreateDirectory(fileName.Directory!.FullName);
        ;
        File.WriteAllBytes(fileName.FullName, bytes);
        _logger.LogInformation("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, address);
    }
}