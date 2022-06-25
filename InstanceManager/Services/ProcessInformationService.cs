using System;
using System.Diagnostics;

namespace InstanceManager.Services;

public class ProcessInformationService
{
    public long GetProcessRam(Process process)
    {
        return process.WorkingSet64;
    }
}