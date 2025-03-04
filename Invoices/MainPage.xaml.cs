using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Invoices;

public partial class MainPage : ContentPage
{
    private readonly ILogger<MainPage>? _logger;

    public MainPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            HandleException(ex, "InitializeComponent");
        }
    }

    public MainPage(ILogger<MainPage> logger) : this()
    {
        _logger = logger;
    }

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();
        }
        catch (Exception ex)
        {
            HandleException(ex, "OnAppearing");
        }
    }

    protected override void OnDisappearing()
    {
        try
        {
            base.OnDisappearing();
        }
        catch (Exception ex)
        {
            HandleException(ex, "OnDisappearing");
        }
    }

    private void HandleException(Exception ex, string source)
    {
        // Log to debug output
        Debug.WriteLine($"MAINPAGE ERROR IN: {source}");
        Debug.WriteLine($"Exception: {ex.GetType().Name}");
        Debug.WriteLine($"Message: {ex.Message}");
        Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

        if (ex.InnerException != null)
        {
            Debug.WriteLine($"Inner Exception: {ex.InnerException.GetType().Name}");
            Debug.WriteLine($"Inner Exception Message: {ex.InnerException.Message}");
            Debug.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
        }

        // Log to logger if available
        _logger?.LogError(ex, "Exception in MainPage.{Source}", source);

        // Display a user-friendly message (could be localized in a real app)
        try
        {
            Dispatcher.Dispatch(async void () =>
            {
                await DisplayAlert("Application Error",
                    "An error occurred. Please try again or contact support if the problem persists.",
                    "OK");
            });
        }
        catch
        {
            // Fail silently if we can't display the alert
        }
    }
}