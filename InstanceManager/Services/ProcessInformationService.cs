using System;
using System.Diagnostics;

namespace InstanceManager.Services;

public class ProcessInformationService
{
    public long GetProcessRam(Process process)
    {
        Console.WriteLine("PagedMemorySize64: " + process.PagedMemorySize64);
        Console.WriteLine("PrivateMemorySize64: " + process.PrivateMemorySize64);
        Console.WriteLine("VirtualMemorySize64: " + process.VirtualMemorySize64);
        Console.WriteLine("NonpagedSystemMemorySize64: " + process.NonpagedSystemMemorySize64);
        Console.WriteLine("PagedSystemMemorySize64: " + process.PagedSystemMemorySize64);
        Console.WriteLine("PeakPagedMemorySize64: " + process.PeakPagedMemorySize64);
        Console.WriteLine("PeakVirtualMemorySize64: " + process.PeakVirtualMemorySize64);
        Console.WriteLine("PeakWorkingSet64: " + process.PeakWorkingSet64);
        Console.WriteLine("WorkingSet64: " + process.WorkingSet64);
        return 0; //TODO return real RAM 
    }
}