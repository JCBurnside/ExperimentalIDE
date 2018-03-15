using Avalonia;
using Avalonia.Markup.Xaml;

namespace ExperimentalIDE
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
