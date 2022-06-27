using System.Diagnostics;
using System.IO.Compression;
using DMConnect.Share;
using Domain.Dto.DedicatedMachineDto;
using Domain.Entities.Instances;
using InstanceManager.Services;
using Microsoft.Extensions.Logging;
using ThreadState = System.Threading.ThreadState;

namespace InstanceManager;

public class InstanceClient
{
    private readonly Guid InstanceId;
    private readonly string BuildUrl;
    private readonly string StartScript;
    private readonly DownloadService _downloadService;
    private readonly Thread _thread;
    private readonly IDedicatedMachineHub _hub;
    private readonly CancellationTokenSource _cancellationToken;
    private ILogger<InstanceClient> _logger;

    public InstanceClient(Guid instanceId, 
        string buildUrl, 
        string startScript, 
        DownloadService downloadService, 
        IDedicatedMachineHub hub, 
        ILogger<InstanceClient> logger)
    {
        InstanceId = instanceId;
        BuildUrl = buildUrl;
        StartScript = startScript;
        _downloadService = downloadService;
        _hub = hub;
        _thread = new Thread(Launch);
        _cancellationToken = new CancellationTokenSource();
        _logger = logger;
    }

    public void Start()
    {
        _thread.Start();
    }

    public void Stop()
    {
        _logger.LogInformation("Stop command has been summoned");
        _cancellationToken.Cancel();
        if (_thread.ThreadState != ThreadState.Unstarted)
            _thread.Join();
    }

    private void Launch()
    {
        _hub.InstanceSetState(new InstanceSetStateDto(InstanceId, InstanceState.Installing));
        var fileName = Path.Combine(InstanceId.ToString(), "build.zip");
        _downloadService.DownloadFile(BuildUrl, fileName);
        _logger.LogInformation("Unpacking...");
        ZipFile.ExtractToDirectory(fileName, InstanceId.ToString());

        try
        {
            using var myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = InstanceId.ToString() + '/' + StartScript;
            myProcess.StartInfo.CreateNoWindow = true;

            _logger.LogInformation("Starting thread with output");
            var threadOut = CreateStreamResendingThread(myProcess.StandardOutput,
                line => _hub.InstanceStdOut(new InstanceStdOutDto(InstanceId, line)));
            threadOut.Start();

            _logger.LogInformation("Starting thread with errors");
            var threadError = CreateStreamResendingThread(myProcess.StandardError,
                line => _hub.InstanceStdErr(new InstanceStdErrDto(InstanceId, line)));
            threadError.Start();

            _logger.LogInformation("Starting process...");
            myProcess.Start();
            _hub.InstanceSetState(new InstanceSetStateDto(InstanceId, InstanceState.Running));
            var task = myProcess.WaitForExitAsync();
            task.Wait(_cancellationToken.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static Thread CreateStreamResendingThread(TextReader reader, Action<string> action)
    {
        return new Thread(() =>
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                action.Invoke(line);
            }
        });
    }
}