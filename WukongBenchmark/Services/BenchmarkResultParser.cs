namespace WukongBenchmark.Services;

public class BenchmarkResultParser
{
    public BenchmarkResult Parse(
        string average,
        string maximum,
        string minimum,
        string low5th,
        string usedVram,
        string vram,
        string ram,
        string graphicsPreset,
        string systemText,
        string graphicsText)
    {
        var result = new BenchmarkResult();

        result.AverageFps = average;
        result.MaximumFps = maximum;
        result.MinimumFps = minimum;
        result.Low5thFps = low5th;
        result.usedVram = usedVram;

        result.GameVersion =
            GetValueAfterLabel(systemText, "Game Version");

        result.OperatingSystemVersion =
            GetValueAfterLabel(
                systemText,
                "Operating System Version");

        result.Cpu =
            GetValueAfterLabel(systemText, "CPU");

        result.Gpu =
            GetValueAfterLabel(systemText, "Graphics Card");

        result.GpuDriver =
            GetValueAfterLabel(
                systemText,
                "Graphics Card Driver");

        result.Vram = vram;
        result.Ram = ram;

        result.GraphicsPreset = graphicsPreset;

        result.FrameGeneration =
            GetValueAfterLabel(
                graphicsText,
                "Frame Generation");

        return result;
    }
    private string GetValueAfterLabel(
        string text,
        string label)
    {
        string[] lines =
            text.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            if (!line.Equals(
                label,
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (i + 1 < lines.Length)
            {
                return lines[i + 1].Trim();
            }
        }

        return "Не найдено";
    }
}