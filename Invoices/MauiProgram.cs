using Microsoft.Extensions.Logging;
using Invoices.Services;
using MudBlazor;
using MudBlazor.Services;
using System.Diagnostics;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Foundation;

namespace Invoices;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();

        // Register MudBlazor services
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 5000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
        });

        SkiaLoader.Load();

        // Register app services
        builder.Services.AddSingleton<InvoiceConfigurationService>();
        builder.Services.AddSingleton<InvoiceService>();
        builder.Services.AddSingleton<ExceptionHandlerService>();
        builder.Services.AddSingleton<IPdfService, PdfService>();
        
        builder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);

        // Advanced logging configuration to capture all details
        ConfigureLogging(builder);

        return builder.Build();
    }

    private static void ConfigureLogging(MauiAppBuilder builder)
    {
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging
            .AddDebug()
            .SetMinimumLevel(LogLevel.Trace); // Set to capture all logs in debug mode
#else
        builder.Logging
            .AddDebug()
            .SetMinimumLevel(LogLevel.Warning);  // Set to only capture important logs in release mode
#endif

        // Configure logging to capture all exception details
        builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
        builder.Logging.AddFilter("System", LogLevel.Warning);
        builder.Logging.AddFilter("Invoices", LogLevel.Debug);

        // Register a custom logger for file logging (optional)
        var logsPath = new NSFileManager().GetUrls(NSSearchPathDirectory.ApplicationDirectory, NSSearchPathDomain.User)[0]
            .Path;
        builder.Services.AddLogging(logging => logging.AddProvider(new FileLoggerProvider(logsPath!)));
    }
}

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _path;

    public FileLoggerProvider(string path)
    {
        _path = path;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_path, categoryName);
    }

    public void Dispose()
    {
    }

    private class FileLogger : ILogger
    {
        private readonly string _path;
        private readonly string _categoryName;

        public FileLogger(string path, string categoryName)
        {
            _path = path;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var logFile = Path.Combine(_path, $"log-{DateTime.Now:yyyy-MM-dd}.txt");
            var message = formatter(state, exception);

            try
            {
                File.AppendAllText(logFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {message}\n");

                if (exception != null)
                {
                    File.AppendAllText(logFile, $"Exception: {exception.GetType().Name}\n");
                    File.AppendAllText(logFile, $"Message: {exception.Message}\n");
                    File.AppendAllText(logFile, $"Stack Trace: {exception.StackTrace}\n");

                    if (exception.InnerException != null)
                    {
                        File.AppendAllText(logFile, $"Inner Exception: {exception.InnerException.GetType().Name}\n");
                        File.AppendAllText(logFile, $"Inner Exception Message: {exception.InnerException.Message}\n");
                        File.AppendAllText(logFile,
                            $"Inner Exception Stack Trace: {exception.InnerException.StackTrace}\n");
                    }
                }
            }
            catch
            {
                // Fail silently if we can't write to the log file
            }
        }
    }
}