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
    private readonly ILogger<InstanceClient> _logger;
    private Process? _process;

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
        _thread = new Thread(Run);
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
        if (_process is not null && !_process.HasExited)
            _process.Kill();
        if (_thread.ThreadState != ThreadState.Unstarted)
            _thread.Join();
    }

    private void Run()
    {
        _hub.InstanceSetState(new InstanceSetStateDto(InstanceId, InstanceState.Installing));
        var fileName = new FileInfo(Path.Combine("Download", $"{InstanceId}.zip"));
        _downloadService.DownloadFile(BuildUrl, fileName);
        _logger.LogInformation("Unpacking...");
        ZipFile.ExtractToDirectory(fileName.FullName, InstanceId.ToString(), true);

        _process = new Process();
        _process.StartInfo.WorkingDirectory = InstanceId.ToString();
        _process.StartInfo.FileName = Path.Combine(_process.StartInfo.WorkingDirectory, StartScript);
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.RedirectStandardError = true;
        _process.Start();

        var threadOut = CreateStreamResendingThread(_process.StandardOutput,
            line => _hub.InstanceStdOut(new InstanceStdOutDto(InstanceId, line)));
        threadOut.Start();

        var threadError = CreateStreamResendingThread(_process.StandardError,
            line => _hub.InstanceStdErr(new InstanceStdErrDto(InstanceId, line)));
        threadError.Start();

        _hub.InstanceSetState(new InstanceSetStateDto(InstanceId, InstanceState.Running));

        _process.WaitForExit();
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