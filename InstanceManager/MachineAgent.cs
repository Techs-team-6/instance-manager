using System.IO.Compression;
using DMConnect.Share;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.DownloadFiles;

namespace InstanceManager;

public class MachineAgent : IDedicatedMachineAgent
{
    // remote dedicated machine 

    private Download _download;

    public MachineAgent()
    {
        _download = new Download();
    }

    public void StartInstance(StartInstanceDto dto)
    {
        string fileName = dto.InstanceId.ToString() + "/build.zip";
        _download.DownloadFile(dto.BuildUrl, fileName);
        ZipFile.ExtractToDirectory(fileName,dto.InstanceId.ToString());
    }
}