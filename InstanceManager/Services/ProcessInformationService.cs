using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace InstanceManager.Services;

public class ProcessInformationService
{
    private readonly ILogger<ProcessInformationService> _logger;

    public ProcessInformationService(ILogger<ProcessInformationService> logger)
    {
        _logger = logger;
    }
  
    public long GetProcessRam(Process process)
    {
        _logger.LogInformation("Getting process RAM...");
        return process.WorkingSet64;
    }
}