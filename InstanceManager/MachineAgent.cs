using DMConnect.Share;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.Services;
using Microsoft.Extensions.Logging;

namespace InstanceManager;

public class MachineAgent : IDedicatedMachineAgent
{
    private readonly DownloadService _downloadService;
    private readonly IDedicatedMachineHub _hub;
    private readonly Dictionary<Guid, InstanceClient> _instanceClients = new();
    private ILoggerFactory _loggerFactory;
    private ILogger<MachineAgent> _logger;

    public MachineAgent(DownloadService downloadService, IDedicatedMachineHub hub, ILoggerFactory loggerFactory)
    {
        _downloadService = downloadService;
        _hub = hub;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<MachineAgent>();
    }

    public void StartInstance(StartInstanceDto dto)
    {
       
        if (_instanceClients.TryGetValue(dto.InstanceId, out var instanceClient))
        {
            _logger.LogInformation("Stopping instance client due to it being started already");
            instanceClient.Stop();
        }
        
        var client = new InstanceClient(dto.InstanceId, dto.BuildUrl, dto.StartScript, _downloadService, _hub, _loggerFactory.CreateLogger<InstanceClient>());
        _instanceClients[dto.InstanceId] = client; 
        _logger.LogInformation("Starting instance client");
        client.Start();
    }
}