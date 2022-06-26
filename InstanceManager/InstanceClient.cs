using System.Diagnostics;
using System.IO.Compression;
using DMConnect.Share;
using Domain.Dto.DedicatedMachineDto;
using InstanceManager.Services;
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

    public InstanceClient(Guid instanceId, string buildUrl, string startScript, DownloadService downloadService,
        IDedicatedMachineHub hub)
    {
        InstanceId = instanceId;
        BuildUrl = buildUrl;
        StartScript = startScript;
        _downloadService = downloadService;
        _hub = hub;
        _thread = new Thread(Launch);
        _cancellationToken = new CancellationTokenSource();
    }

    public void Start()
    {
        _thread.Start();
    }

    public void Stop()
    {
        _cancellationToken.Cancel();
        if (_thread.ThreadState != ThreadState.Unstarted)
            _thread.Join();
    }

    private void Launch()
    {
        var fileName = Path.Combine(InstanceId.ToString(), "build.zip");
        DownloadService.DownloadFile(BuildUrl, fileName);
        ZipFile.ExtractToDirectory(fileName, InstanceId.ToString());

        try
        {
            using var myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = InstanceId.ToString() + '/' + StartScript;
            myProcess.StartInfo.CreateNoWindow = true;

            var threadOut = CreateStreamResendingThread(myProcess.StandardOutput,
                line => _hub.InstanceStdOut(new InstanceStdOutDto(InstanceId, line)));
            threadOut.Start();

            var threadError = CreateStreamResendingThread(myProcess.StandardError,
                line => _hub.InstanceStdErr(new InstanceStdErrDto(InstanceId, line)));
            threadError.Start();

            myProcess.Start();
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