using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace ExperimentalIDE
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
    
}
