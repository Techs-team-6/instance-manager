using System.Net;
using DMConnect.Client;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.Services;

namespace InstanceManager;

public static class Program
{
    public static void Main(string[] args)
    {
        var downloadService = new DownloadService();
        var machineAgent = new MachineAgent(downloadService);
        var endPoint = new IPEndPoint(Dns.GetHostByName(args[0]).AddressList[0], int.Parse(args[1]));
        var token = args[2];
        var fileName = "machineId.txt";
        RemoteDedicatedMachineHub client;
        if (File.Exists(fileName))
        {
            var id = Guid.Parse(File.ReadAllText(fileName));
            client = new RemoteDedicatedMachineHub(endPoint, machineAgent, new AuthDto(id, token));
        }
        else
        {
            client = new RemoteDedicatedMachineHub(endPoint, machineAgent,
                new RegisterDto(token, Environment.OSVersion.ToString(), "Description is cool, but no"));
            File.WriteAllText(fileName, client.MachineId.ToString());
        }

        machineAgent.Hub = client;
        client.Start();
    }
}