using System.Diagnostics;
using InstanceManager.DownloadFiles;
using InstanceManager.Procesess;
using InstanceManager.SystemInfo;

namespace InstanceManager
{
    class MyClass
    {
        static void Main()
        {
            Download download = new Download();
            download.DownloadFile("https://vscode.ru/filesForArticles/test.docx",@"C:\Users\cuatr\Desktop\Skype.docx");
            
            ProcesInformation procesInformation = new ProcesInformation();
            procesInformation.GetProcessInformation();

            var client = new MemoryMetricsClient();
            var metrics = client.GetMetrics();
            
            Console.WriteLine("Операционная система (номер версии):  {0}", Environment.OSVersion);
            Console.WriteLine("Total: " + metrics.Total);
            Console.WriteLine("Used : " + metrics.Used);
            Console.WriteLine("Free : " + metrics.Free);
        }
    }
}