using Foundation;

namespace Invoices;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        MacSkiaLoader.Load();
        return MauiProgram.CreateMauiApp();
    }
}