using System.Net;
using DMConnect.Client;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.Services;
using Microsoft.Extensions.Logging;

namespace InstanceManager;

public static class Program
{
    public static void Main(string[] args)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        var downloadService = new DownloadService(loggerFactory.CreateLogger<DownloadService>());
        var memoryMetricsService = new MemoryMetricsService(loggerFactory.CreateLogger<MemoryMetricsService>());
        var processInformationService =
            new ProcessInformationService(loggerFactory.CreateLogger<ProcessInformationService>());

        var endPoint = new IPEndPoint(Dns.GetHostByName(args[0]).AddressList[0], int.Parse(args[1]));

        var token = args[2];
        var label = Environment.OSVersion.ToString();
        var description = "Description is cool, but no";

        var hubClient = new DedicatedMachineHubClient(endPoint, new RegisterDto(token, label, description));
        var machineAgent = new MachineAgent(downloadService, hubClient, loggerFactory);
        hubClient.SetMachineAgent(machineAgent);

        hubClient.Start();
    }
}