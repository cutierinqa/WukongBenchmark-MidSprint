using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace WukongBenchmark.Services;

public class BenchmarkAutomationService
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(
        IntPtr hWnd,
        int nCmdShow);

    private const int SW_RESTORE = 9;

    public bool StartAndWaitForWindow()
    {
        var launcher = new BenchmarkToolService();

        if (!launcher.IsInstalled())
        {
            throw new FileNotFoundException(
                "Benchmark Tool не найден.",
                launcher.GetPath());
        }

        launcher.StartBenchmarkTool();

        for (int i = 0; i < 30; i++)
        {
            Thread.Sleep(1000);

            var process = Process
                .GetProcessesByName("b1-Win64-Shipping")
                .FirstOrDefault();

            if (process == null)
                continue;

            if (process.MainWindowHandle == IntPtr.Zero)
                continue;

            var handle = process.MainWindowHandle;

            ShowWindow(handle, SW_RESTORE);
            SetForegroundWindow(handle);

            return true;
        }

        return false;
    }
    public void CaptureWukongWindow()
    {
        var process = Process
            .GetProcessesByName("b1-Win64-Shipping")
            .FirstOrDefault();

        if (process == null)
        {
            throw new Exception(
                "Процесс Benchmark Tool не найден.");
        }

        if (process.MainWindowHandle == IntPtr.Zero)
        {
            throw new Exception(
                "Окно Benchmark Tool не найдено.");
        }

        var handle = process.MainWindowHandle;

        ShowWindow(handle, SW_RESTORE);
        SetForegroundWindow(handle);

        Thread.Sleep(500);

        if (!GetWindowRect(handle, out RECT rect))
        {
            throw new Exception(
                "Не удалось получить размеры окна.");
        }

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        if (width <= 0 || height <= 0)
        {
            throw new Exception(
                "Некорректный размер окна.");
        }

        using var bitmap =
    new System.Drawing.Bitmap(width, height);

        using var graphics =
            System.Drawing.Graphics.FromImage(bitmap);

        graphics.CopyFromScreen(
            rect.Left,
            rect.Top,
            0,
            0,
            new System.Drawing.Size(width, height));

        string screenshotsFolder = Path.Combine(
    AppContext.BaseDirectory,
    "Screenshots");

        Directory.CreateDirectory(screenshotsFolder);

        string imagePath = Path.Combine(
            screenshotsFolder,
            "wukong_test.png");

        bitmap.Save(
            imagePath,
            System.Drawing.Imaging.ImageFormat.Png);
    }
    public string GetWukongUiElements()
    {
        var process = Process
            .GetProcessesByName("b1-Win64-Shipping")
            .FirstOrDefault();

        if (process == null)
        {
            return "Процесс b1-Win64-Shipping не найден.";
        }

        if (process.MainWindowHandle == IntPtr.Zero)
        {
            return "У процесса нет главного окна.";
        }

        var automation = new FlaUI.UIA3.UIA3Automation();

        using (automation)
        {
            var window =
                automation.FromHandle(process.MainWindowHandle);

            var elements = window.FindAllDescendants();

            var result =
                new System.Text.StringBuilder();

            result.AppendLine(
                $"Window: {window.Name}");
            result.AppendLine(
                $"Elements: {elements.Length}");
            result.AppendLine();

            foreach (var element in elements)
            {
                try
                {
                    var name = element.Name;
                    var controlType = element.ControlType;

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        result.AppendLine(
                            $"{controlType}: {name}");
                    }
                }
                catch
                {
                    
                }
            }

            return result.ToString();
        }
    }

    public void TestClick()
    {
        var process = Process
            .GetProcessesByName("b1-Win64-Shipping")
            .FirstOrDefault();

        if (process == null)
        {
            throw new Exception(
                "Процесс b1-Win64-Shipping не найден.");
        }

        if (process.MainWindowHandle == IntPtr.Zero)
        {
            throw new Exception(
                "Окно Benchmark Tool не найдено.");
        }

        var handle = process.MainWindowHandle;

        if (!GetWindowRect(handle, out RECT rect))
        {
            throw new Exception(
                "Не удалось получить координаты окна.");
        }

        int x = rect.Left + 500;
        int y = rect.Top + 500;

        SetForegroundWindow(handle);

        Thread.Sleep(500);

        SetCursorPos(x, y);

        Thread.Sleep(200);

        mouse_event(
            MOUSEEVENTF_LEFTDOWN,
            0,
            0,
            0,
            UIntPtr.Zero);

        mouse_event(
            MOUSEEVENTF_LEFTUP,
            0,
            0,
            0,
            UIntPtr.Zero);
    }
    public string GetMousePositionRelativeToWukong()
    {
        var process = Process
            .GetProcessesByName("b1-Win64-Shipping")
            .FirstOrDefault();

        if (process == null)
        {
            return "Процесс b1-Win64-Shipping не найден.";
        }

        if (process.MainWindowHandle == IntPtr.Zero)
        {
            return "Окно Benchmark Tool не найдено.";
        }

        if (!GetWindowRect(
            process.MainWindowHandle,
            out RECT rect))
        {
            return "Не удалось получить координаты окна.";
        }

        if (!GetCursorPos(out POINT cursor))
        {
            return "Не удалось получить координаты мыши.";
        }

        int relativeX = cursor.X - rect.Left;
        int relativeY = cursor.Y - rect.Top;

        return
            $"Координаты мыши:\n\n" +
            $"Экран: X={cursor.X}, Y={cursor.Y}\n\n" +
            $"Относительно окна b1:\n" +
            $"X={relativeX}, Y={relativeY}";
    }
    public void RunBenchmarkClicks()
    {
        var process = Process
            .GetProcessesByName("b1-Win64-Shipping")
            .FirstOrDefault();

        if (process == null)
            throw new Exception(
                "Процесс b1-Win64-Shipping не найден.");

        if (process.MainWindowHandle == IntPtr.Zero)
            throw new Exception(
                "Окно Benchmark Tool не найдено.");

        var handle = process.MainWindowHandle;

        if (!GetWindowRect(handle, out RECT rect))
            throw new Exception(
                "Не удалось получить координаты окна.");

        ShowWindow(handle, SW_RESTORE);
        SetForegroundWindow(handle);

        Thread.Sleep(500);

        int centerX = rect.Left + (rect.Right - rect.Left) / 2;
        int centerY = rect.Top + (rect.Bottom - rect.Top) / 2;

        SetCursorPos(centerX, centerY);

        Thread.Sleep(200);

        mouse_event(
            MOUSEEVENTF_LEFTDOWN,
            0,
            0,
            0,
            UIntPtr.Zero);

        mouse_event(
            MOUSEEVENTF_LEFTUP,
            0,
            0,
            0,
            UIntPtr.Zero);

        Thread.Sleep(700);

        int benchmarkX = rect.Left + 186;
        int benchmarkY = rect.Top + 487;

        SetCursorPos(benchmarkX, benchmarkY);

        Thread.Sleep(200);

        mouse_event(
            MOUSEEVENTF_LEFTDOWN,
            0,
            0,
            0,
            UIntPtr.Zero);

        mouse_event(
            MOUSEEVENTF_LEFTUP,
            0,
            0,
            0,
            UIntPtr.Zero);

        Thread.Sleep(700);

        int confirmX = rect.Left + 829;
        int confirmY = rect.Top + 641;

        SetCursorPos(confirmX, confirmY);

        Thread.Sleep(200);

        mouse_event(
            MOUSEEVENTF_LEFTDOWN,
            0,
            0,
            0,
            UIntPtr.Zero);

        mouse_event(
            MOUSEEVENTF_LEFTUP,
            0,
            0,
            0,
            UIntPtr.Zero);
    }
    public void StartBenchmarkAutomatically()
    {
        var launcher = new BenchmarkToolService();

        if (!launcher.IsInstalled())
        {
            throw new FileNotFoundException(
                "Benchmark Tool не найден.",
                launcher.GetPath());
        }

        launcher.StartBenchmarkTool();
        Process? process = null;

        for (int i = 0; i < 30; i++)
        {
            Thread.Sleep(1000);

            process = Process
                .GetProcessesByName("b1-Win64-Shipping")
                .FirstOrDefault(p =>
                    p.MainWindowHandle != IntPtr.Zero);

            if (process != null)
                break;
        }

        if (process == null)
        {
            throw new Exception(
                "Окно Benchmark Tool не появилось.");
        }

        var handle = process.MainWindowHandle;

        if (!GetWindowRect(handle, out RECT rect))
        {
            throw new Exception(
                "Не удалось получить координаты окна.");
        }

        ShowWindow(handle, SW_RESTORE);
        SetForegroundWindow(handle);

        Thread.Sleep(26000);

        int centerX =
            rect.Left + (rect.Right - rect.Left) / 2;

        int centerY =
            rect.Top + (rect.Bottom - rect.Top) / 2;

        ClickAt(centerX, centerY);

        Thread.Sleep(700);

        int benchmarkX = rect.Left + 186;
        int benchmarkY = rect.Top + 487;

        ClickAt(benchmarkX, benchmarkY);

        Thread.Sleep(700);

        int confirmX = rect.Left + 829;
        int confirmY = rect.Top + 641;

        ClickAt(confirmX, confirmY);

        Thread.Sleep(170000);

        Thread.Sleep(25000);

        CaptureWukongWindow();
    }

    private void ClickAt(int x, int y)
    {
        SetCursorPos(x, y);

        Thread.Sleep(200);

        mouse_event(
            MOUSEEVENTF_LEFTDOWN,
            0,
            0,
            0,
            UIntPtr.Zero);

        mouse_event(
            MOUSEEVENTF_LEFTUP,
            0,
            0,
            0,
            UIntPtr.Zero);
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(
    IntPtr hWnd,
    out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(
    int X,
    int Y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(
        uint dwFlags,
        uint dx,
        uint dy,
        uint dwData,
        UIntPtr dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT point);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }
}