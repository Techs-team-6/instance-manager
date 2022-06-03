using System.Diagnostics;

namespace InstanceManager.Procesess;

public class ProcesInformation
{
    public void StartProcess(string path)
    {
        try
        {
            using (Process myProcess = new Process())
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = path;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    
    public void GetProcessInformation(string path)
        {
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo.FileName = path;
                    myProcess.StartInfo.CreateNoWindow = true;
                    var streamReader = myProcess.StandardOutput;
                    myProcess.Start();
                    string? line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                    Console.WriteLine("PagedMemorySize64: "+myProcess.PagedMemorySize64);
                    Console.WriteLine("PrivateMemorySize64: "+myProcess.PrivateMemorySize64);
                    Console.WriteLine("VirtualMemorySize64: "+myProcess.VirtualMemorySize64);
                    Console.WriteLine("NonpagedSystemMemorySize64: "+myProcess.NonpagedSystemMemorySize64);
                    Console.WriteLine("PagedSystemMemorySize64: "+myProcess.PagedSystemMemorySize64);
                    Console.WriteLine("PeakPagedMemorySize64: "+myProcess.PeakPagedMemorySize64);
                    Console.WriteLine("PeakVirtualMemorySize64: "+myProcess.PeakVirtualMemorySize64);
                    Console.WriteLine("PeakWorkingSet64: "+myProcess.PeakWorkingSet64);
                    Console.WriteLine("WorkingSet64: "+myProcess.WorkingSet64);
                    // This code assumes the process you are starting will terminate itself.
                    // Given that it is started without a window so you cannot terminate it
                    // on the desktop, it must terminate itself or you can do it programmatically
                    // from this application using the Kill method.
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //Process.Start(@"C:\Users\cuatr\Desktop\Untitled2.exe");
        }
}