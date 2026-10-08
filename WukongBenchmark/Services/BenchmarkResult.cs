namespace WukongBenchmark.Services;

public class BenchmarkResult
{
    public string AverageFps { get; set; } = "";
    public string MaximumFps { get; set; } = "";
    public string MinimumFps { get; set; } = "";
    public string Low5thFps { get; set; } = "";
    public string usedVram { get; set; } = "";

    public string GameVersion { get; set; } = "";
    public string OperatingSystemVersion { get; set; } = "";
    public string Cpu { get; set; } = "";
    public string Gpu { get; set; } = "";
    public string GpuDriver { get; set; } = "";
    public string Vram { get; set; } = "";
    public string Ram { get; set; } = "";

    public string GraphicsPreset { get; set; } = "";
    public string FrameGeneration { get; set; } = "";
}