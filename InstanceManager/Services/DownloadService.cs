using System.Net;

namespace InstanceManager.Services;

public class DownloadService
{
    public void DownloadFile(string address, string fileName)
    {
        WebClient myWebClient = new WebClient();
        var pathName = Path.Combine("Download", fileName);
        //Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n", fileName, address);
        myWebClient.DownloadFileAsync(new Uri(address), fileName);
        //Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, address);
    }
}