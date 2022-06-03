using System.Diagnostics;
using System.IO.Compression;
using DMConnect.Client;
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

    public IDedicatedMachineHub Hub { get; set; }

    public void StartInstance(StartInstanceDto dto)
    {
        string fileName = dto.InstanceId.ToString() + "/build.zip";
        _download.DownloadFile(dto.BuildUrl, fileName);
        ZipFile.ExtractToDirectory(fileName, dto.InstanceId.ToString());

        try
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = dto.InstanceId.ToString() + '/' + dto.StartScript;
                myProcess.StartInfo.CreateNoWindow = true;
                var streamReader = myProcess.StandardOutput;
                myProcess.Start();
                string? line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
                /*Console.WriteLine("PagedMemorySize64: "+myProcess.PagedMemorySize64);
                Console.WriteLine("PrivateMemorySize64: "+myProcess.PrivateMemorySize64);
                Console.WriteLine("VirtualMemorySize64: "+myProcess.VirtualMemorySize64);
                Console.WriteLine("NonpagedSystemMemorySize64: "+myProcess.NonpagedSystemMemorySize64);
                Console.WriteLine("PagedSystemMemorySize64: "+myProcess.PagedSystemMemorySize64);
                Console.WriteLine("PeakPagedMemorySize64: "+myProcess.PeakPagedMemorySize64);
                Console.WriteLine("PeakVirtualMemorySize64: "+myProcess.PeakVirtualMemorySize64);
                Console.WriteLine("PeakWorkingSet64: "+myProcess.PeakWorkingSet64);
                Console.WriteLine("WorkingSet64: "+myProcess.WorkingSet64);*/
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}