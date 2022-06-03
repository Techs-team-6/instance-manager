using System.Diagnostics;
using System.Net;
using DMConnect.Client;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.DownloadFiles;
using InstanceManager.Procesess;
using InstanceManager.SystemInfo;

namespace InstanceManager
{
    class MyClass
    {
        static void Main(string[] args)
        {
            var machineAgent = new MachineAgent();
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

            /*ProcesInformation procesInformation = new ProcesInformation();
            procesInformation.GetProcessInformation();

            var client = new MemoryMetricsClient();
            var metrics = client.GetMetrics();
            
            Console.WriteLine("Операционная система (номер версии):  {0}", Environment.OSVersion);
            Console.WriteLine("Total: " + metrics.Total);
            Console.WriteLine("Used : " + metrics.Used);
            Console.WriteLine("Free : " + metrics.Free);*/
        }
    }
}