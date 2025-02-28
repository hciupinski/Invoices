using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Invoices;

public partial class App : Application
{
    private readonly ILogger<App>? _logger;

    public App()
    {
        InitializeComponent();

        // Set up global exception handling
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var exception = args.ExceptionObject as Exception;
            LogUnhandledException(exception, "AppDomain.CurrentDomain.UnhandledException");
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            LogUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");
            args.SetObserved(); // Prevent the application from terminating
        };
    }

    public App(ILogger<App> logger) : this()
    {
        _logger = logger;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        try
        {
            return new Window(new MainPage()) { Title = "Invoices" };
        }
        catch (Exception ex)
        {
            LogUnhandledException(ex, "CreateWindow");
            throw; // Re-throw after logging
        }
    }

    private void LogUnhandledException(Exception? exception, string source)
    {
        if (exception == null) return;

        // Log to debug output
        Debug.WriteLine($"CRITICAL ERROR FROM: {source}");
        Debug.WriteLine($"Exception: {exception.GetType().Name}");
        Debug.WriteLine($"Message: {exception.Message}");
        Debug.WriteLine($"Stack Trace: {exception.StackTrace}");

        if (exception.InnerException != null)
        {
            Debug.WriteLine($"Inner Exception: {exception.InnerException.GetType().Name}");
            Debug.WriteLine($"Inner Exception Message: {exception.InnerException.Message}");
            Debug.WriteLine($"Inner Exception Stack Trace: {exception.InnerException.StackTrace}");
        }

        // Log to logger if available
        _logger?.LogCritical(exception, "Unhandled exception from {Source}", source);
        
        // In a real app, you might want to save to a log file or send to a remote service
    }
}
