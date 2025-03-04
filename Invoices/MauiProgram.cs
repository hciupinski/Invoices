using Microsoft.Extensions.Logging;
using Invoices.Services;
using MudBlazor;
using MudBlazor.Services;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;

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
    }
}