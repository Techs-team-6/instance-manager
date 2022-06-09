﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using InstanceManager.Entities;

namespace InstanceManager.Services;

public class MemoryMetricsService
{
    public MemoryMetrics GetMetrics()
    {
        if (IsUnix())
        {
            return GetUnixMetrics();
        }

        return GetWindowsMetrics();
    }

    private bool IsUnix()
    {
        var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        return isUnix;
    }

    private MemoryMetrics GetWindowsMetrics()
    {
        var output = "";

        var info = new ProcessStartInfo();
        info.FileName = "wmic";
        info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
        info.RedirectStandardOutput = true;

        using (var process = Process.Start(info))
        {
            output = process!.StandardOutput.ReadToEnd();
        }

        var lines = output.Trim().Split("\n");
        var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
        var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics();
        metrics.Total = long.Parse(totalMemoryParts[1]) * 1024;
        metrics.Free = long.Parse(freeMemoryParts[1]) * 1024;
        metrics.Used = metrics.Total - metrics.Free;

        return metrics;
    }

    private MemoryMetrics GetUnixMetrics()
    {
        var output = "";

        var info = new ProcessStartInfo("free -m");
        info.FileName = "/bin/bash";
        info.Arguments = "-c \"free -m\"";
        info.RedirectStandardOutput = true;

        using (var process = Process.Start(info))
        {
            output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
        }

        var lines = output.Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

        var metrics = new MemoryMetrics();
        metrics.Total = long.Parse(memory[1]);
        metrics.Used = long.Parse(memory[2]);
        metrics.Free = long.Parse(memory[3]);

        return metrics;
    }
}