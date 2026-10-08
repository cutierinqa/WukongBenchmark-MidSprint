namespace WukongBenchmark.Services;

public class BenchmarkHistoryEntry
{
    public DateTime Date { get; set; }
    public string TestDuration { get; set; } = "";

    public string Cpu { get; set; } = "";
    public string Gpu { get; set; } = "";
    public string Ram { get; set; } = "";

    public string GameVersion { get; set; } = "";
    public string OperatingSystemVersion { get; set; } = "";
    public string GpuDriver { get; set; } = "";
    public string Vram { get; set; } = "";
    public string FrameGeneration { get; set; } = "";

    public string CpuAverageFps { get; set; } = "";
    public string CpuMaximumFps { get; set; } = "";
    public string CpuMinimumFps { get; set; } = "";
    public string CpuLow5thFps { get; set; } = "";
    public string CpuUsedVram { get; set; } = "";

    public string GpuAverageFps { get; set; } = "";
    public string GpuMaximumFps { get; set; } = "";
    public string GpuMinimumFps { get; set; } = "";
    public string GpuLow5thFps { get; set; } = "";
    public string GpuUsedVram { get; set; } = "";

    public string CpuSettings { get; set; } = "";
    public string GpuSettings { get; set; } = "";
}