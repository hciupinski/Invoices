using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Invoices.Services;

public class ExceptionHandlerService
{
    private readonly ILogger<ExceptionHandlerService> _logger;
    private readonly string _logFilePath;

    public ExceptionHandlerService(ILogger<ExceptionHandlerService> logger)
    {
        _logger = logger;
        
        // Set up a log file path in the app's data directory
        var appDataPath = FileSystem.AppDataDirectory;
        _logFilePath = Path.Combine(appDataPath, "logs");
        
        try
        {
            if (!Directory.Exists(_logFilePath))
            {
                Directory.CreateDirectory(_logFilePath);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to create log directory: {ex.Message}");
        }
    }

    public void LogException(Exception exception, string source)
    {
        if (exception == null) return;

        // Log to built-in logger
        _logger.LogError(exception, "Exception from {Source}", source);
        
        // Log to debug output
        Debug.WriteLine($"ERROR FROM: {source}");
        Debug.WriteLine($"Exception: {exception.GetType().Name}");
        Debug.WriteLine($"Message: {exception.Message}");
        Debug.WriteLine($"Stack Trace: {exception.StackTrace}");

        if (exception.InnerException != null)
        {
            Debug.WriteLine($"Inner Exception: {exception.InnerException.GetType().Name}");
            Debug.WriteLine($"Inner Exception Message: {exception.InnerException.Message}");
            Debug.WriteLine($"Inner Exception Stack Trace: {exception.InnerException.StackTrace}");
        }
        
        // Also log to file for post-crash analysis
        try
        {
            var logFile = Path.Combine(_logFilePath, $"error-log-{DateTime.Now:yyyy-MM-dd}.txt");
            
            var logEntry = new StringBuilder();
            logEntry.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR FROM: {source}");
            logEntry.AppendLine($"Exception: {exception.GetType().Name}");
            logEntry.AppendLine($"Message: {exception.Message}");
            logEntry.AppendLine($"Stack Trace: {exception.StackTrace}");
            
            if (exception.InnerException != null)
            {
                logEntry.AppendLine($"Inner Exception: {exception.InnerException.GetType().Name}");
                logEntry.AppendLine($"Inner Exception Message: {exception.InnerException.Message}");
                logEntry.AppendLine($"Inner Exception Stack Trace: {exception.InnerException.StackTrace}");
            }
            
            logEntry.AppendLine("------------------------------------");
            
            File.AppendAllText(logFile, logEntry.ToString());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to write to log file: {ex.Message}");
        }
    }
    
    // Get all log files for displaying to the user or sending to support
    public IEnumerable<string> GetLogFiles()
    {
        try
        {
            return Directory.GetFiles(_logFilePath, "error-log-*.txt")
                .OrderByDescending(f => f);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to get log files: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }
    
    // Read a specific log file
    public string ReadLogFile(string logFilePath)
    {
        try
        {
            return File.ReadAllText(logFilePath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to read log file: {ex.Message}");
            return $"Error reading log file: {ex.Message}";
        }
    }
}
