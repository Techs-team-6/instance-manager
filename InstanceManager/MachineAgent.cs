using System.Diagnostics;
using System.IO.Compression;
using DMConnect.Share;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.Services;

namespace InstanceManager;

public class MachineAgent : IDedicatedMachineAgent
{
    private DownloadService _downloadService;

    public MachineAgent()
    {
        _downloadService = new DownloadService();
    }

    public IDedicatedMachineHub Hub { get; set; }

    public void StartInstance(StartInstanceDto dto)
    {
        string fileName = dto.InstanceId.ToString() + "/build.zip";
        _downloadService.DownloadFile(dto.BuildUrl, fileName);
        ZipFile.ExtractToDirectory(fileName, dto.InstanceId.ToString());

        try
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = dto.InstanceId.ToString() + '/' + dto.StartScript;
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
}