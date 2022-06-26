namespace InstanceManager.Services;

public class DownloadService
{
    public static void DownloadFile(string address, string fileName)
    {
        var client = new HttpClient();
        var pathName = Path.Combine("Download", fileName);

        var bytes = client.GetByteArrayAsync(address).Result;
        File.WriteAllBytes(pathName, bytes);
    }
}