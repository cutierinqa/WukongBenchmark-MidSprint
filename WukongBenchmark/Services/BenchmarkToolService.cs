using System.Diagnostics;
using System.IO;

namespace WukongBenchmark.Services;

public class BenchmarkToolService
{
    private readonly string _benchmarkPath =
        @"D:\SteamLibrary\steamapps\common\Black Myth Wukong Benchmark Tool\b1_benchmark.exe";

    public bool IsInstalled()
    {
        return File.Exists(_benchmarkPath);
    }

    public string GetPath()
    {
        return _benchmarkPath;
    }

    public void StartBenchmarkTool()
    {
        if (!IsInstalled())
        {
            throw new FileNotFoundException(
                "Benchmark Tool не найден.",
                _benchmarkPath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = _benchmarkPath,
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }

    public void PrepareCpuTest()
    {
        string configPath =
            @"D:\SteamLibrary\steamapps\common\Black Myth Wukong Benchmark Tool\b1\Saved\Config\Windows\GameUserSettings.ini";

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException(
                "GameUserSettings.ini не найден.",
                configPath);
        }

        string backupPath = configPath + ".backup";

        File.Copy(
            configPath,
            backupPath,
            true);

        string config = File.ReadAllText(configPath);

        config = ReplaceValue(
            config,
            "sg.ResolutionQuality",
            "66");

        config = ReplaceValue(
            config,
            "sg.ViewDistanceQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.AntiAliasingQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.ShadowQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.GlobalIlluminationQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.RayTracingQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.ReflectionQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.PostProcessQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.TextureQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.EffectsQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.FoliageQuality",
            "0");

        config = ReplaceValue(
            config,
            "sg.ShadingQuality",
            "0");

        config = ReplaceValue(
            config,
            "r.RayTracing.EnableInGame",
            "False");

        config = ReplaceValue(
            config,
            "GSStreamingPoolSize",
            "384");

        File.WriteAllText(
            configPath,
            config);
    }

    private string ReplaceValue(
        string text,
        string key,
        string value)
    {
        var regex = new System.Text.RegularExpressions.Regex(
            $@"^{System.Text.RegularExpressions.Regex.Escape(key)}=.*$",
            System.Text.RegularExpressions.RegexOptions.Multiline);

        if (!regex.IsMatch(text))
        {
            throw new Exception(
                $"Не удалось найти параметр {key} в GameUserSettings.ini");
        }

        return regex.Replace(
            text,
            $"{key}={value}");
    }

    public void PrepareGpuTest()
    {
        string configPath =
            @"D:\SteamLibrary\steamapps\common\Black Myth Wukong Benchmark Tool\b1\Saved\Config\Windows\GameUserSettings.ini";

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException(
                "GameUserSettings.ini не найден.",
                configPath);
        }

        string backupPath = configPath + ".backup";

        File.Copy(configPath, backupPath, true);

        string config = File.ReadAllText(configPath);

        // GPU Test: синематик пресет
        config = ReplaceValue(config, "sg.ResolutionQuality", "100");
        config = ReplaceValue(config, "sg.ViewDistanceQuality", "4");
        config = ReplaceValue(config, "sg.AntiAliasingQuality", "4");
        config = ReplaceValue(config, "sg.ShadowQuality", "4");
        config = ReplaceValue(config, "sg.GlobalIlluminationQuality", "4");
        config = ReplaceValue(config, "sg.RayTracingQuality", "0");
        config = ReplaceValue(config, "sg.ReflectionQuality", "4");
        config = ReplaceValue(config, "sg.PostProcessQuality", "4");
        config = ReplaceValue(config, "sg.TextureQuality", "4");
        config = ReplaceValue(config, "sg.EffectsQuality", "4");
        config = ReplaceValue(config, "sg.FoliageQuality", "4");
        config = ReplaceValue(config, "sg.ShadingQuality", "4");

        config = ReplaceValue(
            config,
            "r.RayTracing.EnableInGame",
            "True");

        config = ReplaceValue(
            config,
            "GSStreamingPoolSize",
            "640");

        File.WriteAllText(configPath, config);
    }

    public void StopBenchmarkTool()
    {
        string[] processNames =
        {
        "b1-Win64-Shipping",
        "b1_benchmark"
    };

        foreach (string processName in processNames)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch
                {
                  
                }
                finally
                {
                    process.Dispose();
                }
            }
        }
    }
}