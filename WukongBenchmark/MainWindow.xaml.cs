using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using WukongBenchmark.Services;

namespace WukongBenchmark;

public partial class MainWindow : Window
{
    private readonly BenchmarkHistoryService _historyService =
    new BenchmarkHistoryService();

    public MainWindow()
    {
        InitializeComponent();

        LoadSystemInfo();
        CheckBenchmarkTool();
        LoadLastResult();
        Loaded += MainWindow_Loaded;
    }
    private async void MainWindow_Loaded(
    object sender,
    RoutedEventArgs e)
    {
        await PlaySplashAnimation();
    }
    private async Task PlaySplashAnimation()
    {
        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(1.0)
        };

        SplashLogo.BeginAnimation(
            UIElement.OpacityProperty,
            fadeIn);

        await Task.Delay(1200);

        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromSeconds(1.0)
        };

        SplashLogo.BeginAnimation(
            UIElement.OpacityProperty,
            fadeOut);

        await Task.Delay(1500);
    }
    private void SetActiveMenuButton(Button activeButton)
    {
        HomeButton.Style = (Style)FindResource("MenuButtonStyle");
        HistoryButton.Style = (Style)FindResource("MenuButtonStyle");

        activeButton.Style = (Style)FindResource("ActiveMenuButtonStyle");
    }
    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        HomeView.Visibility = Visibility.Visible;
        HistoryView.Visibility = Visibility.Collapsed;

        SetActiveMenuButton(HomeButton);
    }
    
    private void HistoryButton_Click(object sender, RoutedEventArgs e)
    {
        HomeView.Visibility = Visibility.Collapsed;
        HistoryView.Visibility = Visibility.Visible;

        SetActiveMenuButton(HistoryButton);

        LoadHistory();
    }
    private void LoadSystemInfo()
    {
        var systemInfo = new SystemInfoService();

        CpuText.Text = systemInfo.GetCpu();
        GpuText.Text = systemInfo.GetGpu();
        RamText.Text = systemInfo.GetRam();
    }

    private void CheckBenchmarkTool()
    {
        var benchmarkTool = new BenchmarkToolService();
    }
    private void LoadHistory()
    {
        HistoryPanel.Children.Clear();

        var history = _historyService.GetHistory();

        if (history.Count == 0)
        {
            HistoryPanel.Children.Add(
                new TextBlock
                {
                    Text = "История пока пуста",
                    Foreground =
                        new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Colors.Gray),
                    FontSize = 16
                });

            return;
        }

        foreach (var entry in history)
        {
            var card = new Border
            {
                Background =
                    new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(
                            27, 31, 39)),

                CornerRadius = new CornerRadius(10),

                Padding = new Thickness(20),

                Margin = new Thickness(0, 0, 0, 15)
            };

            var content = new StackPanel();

            content.Children.Add(
                new TextBlock
                {
                    Text = entry.Date.ToString(
                        "dd.MM.yyyy HH:mm"),

                    Foreground =
                        new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Colors.White),

                    FontSize = 18,

                    FontWeight = FontWeights.SemiBold
                });

            content.Children.Add(
                new TextBlock
                {
                    Text =
                        $"CPU: {entry.Cpu}\n" +
                        $"GPU: {entry.Gpu}\n" +
                        $"RAM: {entry.Ram}",

                    Foreground =
                        new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(
                                139, 146, 163)),

                    FontSize = 13,

                    Margin = new Thickness(0, 10, 0, 15)
                });

            content.Children.Add(
                new TextBlock
                {
                    Text =
                        $"CPU TEST    Средний FPS: {entry.CpuAverageFps}\n" +
                        $"GPU TEST    Средний FPS: {entry.GpuAverageFps}",

                    Foreground =
                        new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Colors.White),

                    FontSize = 15
                });

            content.Children.Add(
                new TextBlock
                {
                    Text =
                        $"\nCPU: {entry.CpuSettings}\n" +
                        $"GPU: {entry.GpuSettings}",

                    Foreground =
                        new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(
                                139, 146, 163)),

                    FontSize = 13,

                    Margin = new Thickness(0, 10, 0, 0)
                });

            card.Child = content;

            HistoryPanel.Children.Add(card);
        }
    }
    private void StartAutomationButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var automation =
                new BenchmarkAutomationService();

            bool started =
                automation.StartAndWaitForWindow();

            if (started)
            {
                MessageBox.Show(
                    "Benchmark Tool запущен.\n\nОкно b1 найдено.",
                    "Автоматизация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "Benchmark Tool запустился, но окно b1 не найдено.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private void PrepareCpuTestButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var benchmarkTool =
                new BenchmarkToolService();
            benchmarkTool.StopBenchmarkTool();
            benchmarkTool.PrepareCpuTest();
            var automation =
                new BenchmarkAutomationService();

            automation.StartBenchmarkAutomatically();

            string imagePath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.Desktop),
                    "wukong_test.png");

            if (!File.Exists(imagePath))
            {
                MessageBox.Show(
                    "Benchmark завершился, " +
                    "но screenshot не найден.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }
            var result =
                ParseBenchmarkResult(imagePath);

            MessageBox.Show(
                $"===== CPU TEST =====\n\n" +

                $"===== FPS =====\n" +
                $"Average: {result.AverageFps}\n" +
                $"Maximum: {result.MaximumFps}\n" +
                $"Minimum: {result.MinimumFps}\n" +
                $"Low 5th: {result.Low5thFps}\n" +
                $"Used VRAM: {result.usedVram} GB\n\n" +

                $"===== SYSTEM =====\n" +
                $"Game Version: {result.GameVersion}\n" +
                $"OS Version: {result.OperatingSystemVersion}\n" +
                $"CPU: {result.Cpu}\n" +
                $"GPU: {result.Gpu}\n" +
                $"GPU Driver: {result.GpuDriver}\n" +
                $"VRAM: {result.Vram}\n" +
                $"RAM: {result.Ram}\n\n" +

                $"===== GRAPHICS =====\n" +
                $"Graphics Preset: {result.GraphicsPreset}\n" +
                $"Frame Generation: {result.FrameGeneration}\n",

                "CPU Test Result",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка CPU теста",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void PrepareGpuTestButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var benchmarkTool =
    new BenchmarkToolService();

            benchmarkTool.StopBenchmarkTool();
            benchmarkTool.PrepareGpuTest();

            var automation =
                new BenchmarkAutomationService();

            automation.StartBenchmarkAutomatically();

            string imagePath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.Desktop),
                    "wukong_test.png");

            if (!File.Exists(imagePath))
            {
                MessageBox.Show(
                    "Скриншот результата не найден.",
                    "GPU Test",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            BenchmarkResult result =
                ParseBenchmarkResult(imagePath);

            MessageBox.Show(
                $"GPU TEST\n\n" +
                $"Average FPS: {result.AverageFps}\n" +
                $"Maximum FPS: {result.MaximumFps}\n" +
                $"Minimum FPS: {result.MinimumFps}\n" +
                $"Low 5th: {result.Low5thFps}\n" +
                $"Used VRAM: {result.usedVram}\n\n" +
                $"GPU: {result.Gpu}\n" +
                $"VRAM: {result.Vram}\n" +
                $"RAM: {result.Ram}\n\n" +
                $"Graphics Preset: {result.GraphicsPreset}\n" +
                $"Frame Generation: {result.FrameGeneration}",
                "GPU Test Result",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка GPU теста",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private void CheckWukongElementsButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var automation =
                new BenchmarkAutomationService();

            string elements =
                automation.GetWukongUiElements();

            MessageBox.Show(
                elements,
                "Элементы Benchmark Tool",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void CheckWindowButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            string imagePath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.Desktop),
                "wukong_test.png");

            if (!File.Exists(imagePath))
            {
                MessageBox.Show(
                    "Файл wukong_test.png не найден.",
                    "OCR",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = ParseBenchmarkResult(imagePath);

            MessageBox.Show(
                $"===== FPS =====\n" +
                $"Average: {result.AverageFps}\n" +
                $"Maximum: {result.MaximumFps}\n" +
                $"Minimum: {result.MinimumFps}\n" +
                $"Low 5th: {result.Low5thFps}\n" +
                $"Used VRAM: {result.usedVram}\n\n" +

                $"===== SYSTEM =====\n" +
                $"Game Version: {result.GameVersion}\n" +
                $"OS Version: {result.OperatingSystemVersion}\n" +
                $"CPU: {result.Cpu}\n" +
                $"GPU: {result.Gpu}\n" +
                $"GPU Driver: {result.GpuDriver}\n" +
                $"VRAM: {result.Vram}\n" +
                $"RAM: {result.Ram}\n\n" +

                $"===== GRAPHICS =====\n" +
                $"Graphics Preset: {result.GraphicsPreset}\n" +
                $"Frame Generation: {result.FrameGeneration}\n" ,
                "Результат парсинга",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка OCR",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private void TestClickButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var automation =
                new BenchmarkAutomationService();

            automation.TestClick();

            MessageBox.Show(
                "Тестовый клик выполнен.",
                "Проверка",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private void GetMousePositionButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var automation =
                new BenchmarkAutomationService();

            string position =
                automation.GetMousePositionRelativeToWukong();

            MessageBox.Show(
                position,
                "Координаты",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private void TestBenchmarkClicksButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var automation =
                new BenchmarkAutomationService();

            automation.RunBenchmarkClicks();

            MessageBox.Show(
                "Два клика выполнены.",
                "Проверка",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private void StartBenchmarkAutomaticallyButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var automation =
                new BenchmarkAutomationService();

            automation.StartBenchmarkAutomatically();

            string imagePath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.Desktop),
                "wukong_test.png");

            if (!File.Exists(imagePath))
            {
                MessageBox.Show(
                    "Benchmark завершился, но screenshot не найден.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = ParseBenchmarkResult(imagePath);

            MessageBox.Show(
                $"===== FPS =====\n" +
                $"Average: {result.AverageFps}\n" +
                $"Maximum: {result.MaximumFps}\n" +
                $"Minimum: {result.MinimumFps}\n" +
                $"Low 5th: {result.Low5thFps}\n" +
                $"Used VRAM: {result.usedVram} GB\n\n" +

                $"===== SYSTEM =====\n" +
                $"Game Version: {result.GameVersion}\n" +
                $"OS Version: {result.OperatingSystemVersion}\n" +
                $"CPU: {result.Cpu}\n" +
                $"GPU: {result.Gpu}\n" +
                $"GPU Driver: {result.GpuDriver}\n" +
                $"VRAM: {result.Vram}\n" +
                $"RAM: {result.Ram}\n\n" +

                $"===== GRAPHICS =====\n" +
                $"Graphics Preset: {result.GraphicsPreset}\n" +
                $"Frame Generation: {result.FrameGeneration}\n",
                "Результат Benchmark",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка запуска Benchmark",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private BenchmarkResult ParseBenchmarkResult(string imagePath)
    {
        var ocr = new OcrService();

        string average = ocr.ReadFirstNumber(
    imagePath,
    60,
    340,
    230,
    100);

        string maximum = ocr.ReadFirstNumber(
            imagePath,
            60,
            460,
            130,
            45);

        string minimum = ocr.ReadFirstNumber(
            imagePath,
            310,
            460,
            130,
            45);

        string low5th = ocr.ReadFirstNumber(
            imagePath,
            60,
            540,
            130,
            45);

        string usedVram = ocr.ReadVramNumber(
            imagePath,
            60,
            620,
            130,
            45);

        string vram = ocr.ReadRegion(
            imagePath,
            600,
            600,
            70,
            45);

        string ram = ocr.ReadRegion(
            imagePath,
            590,
            650,
            70,
            60);

        string graphicsPreset = ocr.ReadRegion(
            imagePath,
            1380,
            425,
            265,
            40);

        string systemInfo = ocr.ReadRegion(
            imagePath,
            590,
            285,
            370,
            380);

        string graphicsSettings = ocr.ReadRegion(
            imagePath,
            960,
            0,
            800,
            800);
        


        var parser = new BenchmarkResultParser();

        return parser.Parse(
            average,
            maximum,
            minimum,
            low5th,
            usedVram,
            vram,
            ram,
            graphicsPreset,
            systemInfo,
            graphicsSettings);
    }

    private void LoadLastResult()
    {
        var history = _historyService.GetHistory();

        if (history.Count == 0)
            return;

        var lastResult = history[0];

        CpuAverageFpsText.Text =
            $"{lastResult.CpuAverageFps} FPS";

        CpuMaximumFpsText.Text =
            $"{lastResult.CpuMaximumFps} FPS";

        CpuMinimumFpsText.Text =
            $"{lastResult.CpuMinimumFps} FPS";

        CpuLow5thFpsText.Text =
            $"{lastResult.CpuLow5thFps} FPS";

        CpuUsedVramText.Text =
            $"{lastResult.CpuUsedVram}";

        GpuAverageFpsText.Text =
            $"{lastResult.GpuAverageFps} FPS";

        GpuMaximumFpsText.Text =
            $"{lastResult.GpuMaximumFps} FPS";

        GpuMinimumFpsText.Text =
            $"{lastResult.GpuMinimumFps} FPS";

        GpuLow5thFpsText.Text =
            $"{lastResult.GpuLow5thFps} FPS";

        GpuUsedVramText.Text =
            $"{lastResult.GpuUsedVram}";

        CpuSettingsText.Text =
            lastResult.CpuSettings;

        GpuSettingsText.Text =
            lastResult.GpuSettings;

        CpuText.Text =
            lastResult.Cpu;

        GpuText.Text =
            lastResult.Gpu;

        RamText.Text =
            lastResult.Ram;
        GameVersionText.Text =
    lastResult.GameVersion;

        OsVersionText.Text =
            lastResult.OperatingSystemVersion;

        GpuDriverText.Text =
            lastResult.GpuDriver;

        VramText.Text =
            lastResult.Vram;

        FrameGenerationText.Text =
            lastResult.FrameGeneration;

        LastTestDateText.Text =
            $"Последний тест: {lastResult.Date:dd.MM.yyyy HH:mm}";

        TestDurationText.Text =
            $" · Время: {lastResult.TestDuration}";
    }
    private void RunFullBenchmarkButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var benchmarkTool = new BenchmarkToolService();

            benchmarkTool.StopBenchmarkTool();
            benchmarkTool.PrepareCpuTest();

            var automation =
                new BenchmarkAutomationService();

            automation.StartBenchmarkAutomatically();

            string cpuImagePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Screenshots",
                    "wukong_test.png");

            if (!File.Exists(cpuImagePath))
            {
                MessageBox.Show(
                    "CPU Test завершился, но screenshot не найден.",
                    "CPU Test",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            BenchmarkResult cpuResult =
                ParseBenchmarkResult(cpuImagePath);

            benchmarkTool.StopBenchmarkTool();
            Thread.Sleep(5000);
            benchmarkTool.PrepareGpuTest();

            automation =
                new BenchmarkAutomationService();

            automation.StartBenchmarkAutomatically();

            string gpuImagePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Screenshots",
                    "wukong_test.png");

            if (!File.Exists(gpuImagePath))
            {
                MessageBox.Show(
                    "GPU Test завершился, но screenshot не найден.",
                    "GPU Test",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            BenchmarkResult gpuResult =
                ParseBenchmarkResult(gpuImagePath);

            CpuAverageFpsText.Text =
                $"{cpuResult.AverageFps} FPS";

            CpuMaximumFpsText.Text =
                $"{cpuResult.MaximumFps} FPS";

            CpuMinimumFpsText.Text =
                $"{cpuResult.MinimumFps} FPS";

            CpuLow5thFpsText.Text =
                $"{cpuResult.Low5thFps} FPS";

            CpuUsedVramText.Text =
                $"{cpuResult.usedVram}";

            CpuSettingsText.Text =
                "Low · Resolution 66% · Ray Tracing Off";

            GpuAverageFpsText.Text =
                $"{gpuResult.AverageFps} FPS";

            GpuMaximumFpsText.Text =
                $"{gpuResult.MaximumFps} FPS";

            GpuMinimumFpsText.Text =
                $"{gpuResult.MinimumFps} FPS";

            GpuLow5thFpsText.Text =
                $"{gpuResult.Low5thFps} FPS";

            GpuUsedVramText.Text =
                $"{gpuResult.usedVram}";

            GpuSettingsText.Text =
                "High · Resolution 100% · Ray Tracing On";

            CpuText.Text =
                gpuResult.Cpu;

            GpuText.Text =
                gpuResult.Gpu;

            RamText.Text =
                gpuResult.Ram;

            GameVersionText.Text =
                gpuResult.GameVersion;

            OsVersionText.Text =
                gpuResult.OperatingSystemVersion;

            GpuDriverText.Text =
                gpuResult.GpuDriver;

            VramText.Text = gpuResult.Vram;

            FrameGenerationText.Text =
                gpuResult.FrameGeneration;

            stopwatch.Stop();

            var testDuration =
                stopwatch.Elapsed.ToString(@"hh\:mm\:ss");

            var historyEntry = new BenchmarkHistoryEntry
            {
                Date = DateTime.Now,
                TestDuration = testDuration,

                Cpu = gpuResult.Cpu,
                Gpu = gpuResult.Gpu,
                Ram = gpuResult.Ram,

                CpuAverageFps = cpuResult.AverageFps,
                CpuMaximumFps = cpuResult.MaximumFps,
                CpuMinimumFps = cpuResult.MinimumFps,
                CpuLow5thFps = cpuResult.Low5thFps,
                CpuUsedVram = cpuResult.usedVram,
                GameVersion = gpuResult.GameVersion,
                OperatingSystemVersion = gpuResult.OperatingSystemVersion,
                GpuDriver = gpuResult.GpuDriver,
                Vram = gpuResult.Vram,
                FrameGeneration = gpuResult.FrameGeneration,

                GpuAverageFps = gpuResult.AverageFps,
                GpuMaximumFps = gpuResult.MaximumFps,
                GpuMinimumFps = gpuResult.MinimumFps,
                GpuLow5thFps = gpuResult.Low5thFps,
                GpuUsedVram = gpuResult.usedVram,

                CpuSettings =
        "Low · Resolution 66% · Ray Tracing Off",

                GpuSettings =
        "High · Resolution 100% · Ray Tracing On"
            };

            _historyService.Add(historyEntry);

            LastTestDateText.Text =
                $"Последний тест: {historyEntry.Date:dd.MM.yyyy HH:mm}";

            TestDurationText.Text =
                $" · Время: {historyEntry.TestDuration}";

            benchmarkTool.StopBenchmarkTool();

            MessageBox.Show(
                "CPU и GPU тесты завершены.\n\n" +
                "Результаты тестирования получены.",
                "Тест завершён",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ошибка полного теста",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}