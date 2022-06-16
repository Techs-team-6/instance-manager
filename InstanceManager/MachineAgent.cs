using DMConnect.Share;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.Services;

namespace InstanceManager;

public class MachineAgent : IDedicatedMachineAgent
{
    private DownloadService _downloadService;
    private readonly Dictionary<Guid, InstanceClient> _instanceClients = new ();

    public MachineAgent(DownloadService downloadService)
    {
        _downloadService = downloadService;
    }

    public IDedicatedMachineHub Hub { get; set; }
    
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