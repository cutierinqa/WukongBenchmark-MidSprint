using System.IO;
using System.Text.Json;

namespace WukongBenchmark.Services;

public class BenchmarkHistoryService
{
    private readonly string _historyPath;

    public BenchmarkHistoryService()
    {
        string dataFolder = Path.Combine(
            AppContext.BaseDirectory,
            "Data");

        Directory.CreateDirectory(dataFolder);

        _historyPath = Path.Combine(
            dataFolder,
            "benchmark_history.json");
    }

    public List<BenchmarkHistoryEntry> GetHistory()
    {
        if (!File.Exists(_historyPath))
            return new List<BenchmarkHistoryEntry>();

        string json = File.ReadAllText(_historyPath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<BenchmarkHistoryEntry>();

        return JsonSerializer.Deserialize<List<BenchmarkHistoryEntry>>(json)
               ?? new List<BenchmarkHistoryEntry>();
    }

    public void Add(BenchmarkHistoryEntry entry)
    {
        var history = GetHistory();

        history.Insert(0, entry);

        string json = JsonSerializer.Serialize(
            history,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_historyPath, json);
    }
}