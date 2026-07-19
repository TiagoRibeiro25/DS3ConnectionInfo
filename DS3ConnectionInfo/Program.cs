using System;
using System.IO;
using System.Reflection;
using Avalonia;

namespace DS3ConnectionInfo;

class Program
{
    private static readonly string logPath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".",
        "ds3connectioninfo.log");

    [STAThread]
    public static void Main(string[] args)
    {
        try { File.WriteAllText(logPath, $"=== DS3ConnectionInfo started ===\n"); } catch { }

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            try { File.AppendAllText(logPath, $"FATAL: {ex.Message}\n{ex.StackTrace}\n"); } catch { }
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
