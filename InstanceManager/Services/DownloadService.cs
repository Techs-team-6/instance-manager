using Microsoft.Extensions.Logging;

namespace InstanceManager.Services;

public class DownloadService
{

    private ILogger<DownloadService> _logger;

    public DownloadService(ILogger<DownloadService> logger)
    {
        _logger = logger;
    }

    public void DownloadFile(string address, string fileName)
    {
        var client = new HttpClient();
        var pathName = Path.Combine("Download", fileName);
        _logger.LogInformation("Downloading File \"{0}\" from \"{1}\" .......\n", fileName, address);
        var bytes = client.GetByteArrayAsync(address).Result;
        File.WriteAllBytes(pathName, bytes);
        _logger.LogInformation("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, address);
    }
}