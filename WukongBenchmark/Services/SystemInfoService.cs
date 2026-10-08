using System.Management;

namespace WukongBenchmark.Services;

public class SystemInfoService
{
    public string GetCpu()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT Name FROM Win32_Processor");

        foreach (var item in searcher.Get())
        {
            return item["Name"]?.ToString()?.Trim() ?? "Неизвестно";
        }

        return "Неизвестно";
    }

    public string GetGpu()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT Name FROM Win32_VideoController");

        string? integratedGpu = null;

        foreach (var item in searcher.Get())
        {
            var name = item["Name"]?.ToString();

            if (string.IsNullOrWhiteSpace(name))
                continue;

            // моя дискретная NVIDIA
            if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
            {
                return name;
            }

            // если нет, юзаю встроенную видеокарту
            if (name.Contains("Intel", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("AMD", StringComparison.OrdinalIgnoreCase))
            {
                integratedGpu = name;
            }
        }

        return integratedGpu ?? "Неизвестно";
    }

    public string GetRam()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");

        foreach (var item in searcher.Get())
        {
            if (item["TotalPhysicalMemory"] is ulong bytes)
            {
                double gigabytes = bytes / 1024.0 / 1024.0 / 1024.0;

                return $"{gigabytes:F1} GB";
            }
        }

        return "Неизвестно";
    }
}