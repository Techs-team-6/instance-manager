using System.Diagnostics;
using System.IO.Compression;
using InstanceManager.Services;

namespace InstanceManager;

public class InstanceClient
{
    public readonly Guid InstanceId;
    public readonly string BuildUrl;
    public readonly string StartScript;
    private readonly DownloadService _downloadService;

    public InstanceClient(Guid instanceId, string buildUrl, string startScript, DownloadService downloadService)
    {
        InstanceId = instanceId;
        BuildUrl = buildUrl; 
        StartScript = startScript;
        _downloadService = downloadService;
    }
    
    public void Start()
    {
        string fileName = InstanceId.ToString() + "/build.zip";
        _downloadService.DownloadFile(BuildUrl, fileName);
        ZipFile.ExtractToDirectory(fileName, InstanceId.ToString());

        try
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = InstanceId.ToString() + '/' + StartScript;
                myProcess.StartInfo.CreateNoWindow = true;
                var streamReader = myProcess.StandardOutput;
                myProcess.Start();
                string? line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void Stop()
    {
        //TODO implement
    }
}