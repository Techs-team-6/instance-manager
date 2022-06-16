using DMConnect.Share;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.Services;

namespace InstanceManager;

public class MachineAgent : IDedicatedMachineAgent
{
    private readonly DownloadService _downloadService;
    private readonly IDedicatedMachineHub _hub;
    private readonly Dictionary<Guid, InstanceClient> _instanceClients = new();

    public MachineAgent(DownloadService downloadService, IDedicatedMachineHub hub)
    {
        _downloadService = downloadService;
        _hub = hub;
    }

    public void StartInstance(StartInstanceDto dto)
    {
        if (_instanceClients.TryGetValue(dto.InstanceId, out var instanceClient))
        {
            instanceClient.Stop();
        }

        var client = new InstanceClient(dto.InstanceId, dto.BuildUrl, dto.StartScript, _downloadService);
        _instanceClients[dto.InstanceId] = client;
        client.Start();
    }
}