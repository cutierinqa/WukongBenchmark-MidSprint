using System;
using System.Text;
using System.Runtime.InteropServices;

namespace WukongBenchmark.Services;

public class BenchmarkWindowService
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(
        EnumWindowsProc lpEnumFunc,
        IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int GetWindowText(
        IntPtr hWnd,
        StringBuilder lpString,
        int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(
        IntPtr hWnd);

    private delegate bool EnumWindowsProc(
        IntPtr hWnd,
        IntPtr lParam);

    public string GetOpenWindows()
    {
        var windows = new StringBuilder();

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd))
                return true;

            var title = new StringBuilder(256);

            GetWindowText(
                hWnd,
                title,
                title.Capacity);

            if (title.Length > 0)
            {
                windows.AppendLine(title.ToString());
            }

            return true;

        }, IntPtr.Zero);

        return windows.ToString();
    }

    public bool IsBenchmarkToolRunning()
    {
        bool found = false;

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd))
                return true;

            var title = new StringBuilder(256);

            GetWindowText(
                hWnd,
                title,
                title.Capacity);

            if (title.ToString() == "Wukong Benchmark Suite")
            {
                found = true;
                return false;
            }

            return true;

        }, IntPtr.Zero);

        return found;
    }
}