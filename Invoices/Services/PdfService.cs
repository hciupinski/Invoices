using System.Diagnostics;
using System.Runtime.InteropServices;
using Foundation;
using Invoices.Models;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Invoices.Services;

public interface IPdfService
{
    /// <summary>
    /// Generates a PDF file from the invoice and saves it to the specified path
    /// </summary>
    /// <param name="invoice">The invoice data</param>
    /// <param name="filePath">Path where to save the PDF file</param>
    /// <returns>The absolute path to the saved file</returns>
    Task<string> GeneratePdfAsync(Invoice invoice, string filePath);
    
    /// <summary>
    /// Generates a PDF file from the invoice and opens it for preview
    /// </summary>
    /// <param name="invoice">The invoice data</param>
    /// <returns>Task representing the operation</returns>
    Task PreviewPdfAsync(Invoice invoice);
}

public class PdfService : IPdfService
{
    private readonly IServiceProvider _serviceProvider;

    static PdfService()
    {
        // Set QuestPDF to community license for open-source usage
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public PdfService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public async Task<string> GeneratePdfAsync(Invoice invoice, string filePath)
    {
        // Get invoice configuration service to access stored path
        var configService = _serviceProvider.GetService<InvoiceConfigurationService>();
        var config = configService?.GetConfiguration();
        string storagePath = config?.InvoicesStoragePath ?? 
                             Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Invoices");
        
        // Create an absolute path if necessary
        string absolutePath = Path.IsPathRooted(filePath) 
            ? filePath 
            : Path.Combine(storagePath, filePath);
        
        // Ensure the directory exists
        string? directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Generate the PDF
        var document = new InvoiceDocumentTemplate(invoice);
        
        // Save the file (explicitly do sync for now, making thread-safe)
        document.GeneratePdf(absolutePath);
        
        return absolutePath;
    }
    
    /// <inheritdoc />
    public async Task PreviewPdfAsync(Invoice invoice)
    {
        // Create a temporary filename
        string tempFileName = $"Invoice-{invoice.InvoiceNumber}-{Guid.NewGuid()}.pdf";
        string tempFilePath = Path.Combine(Path.GetTempPath(), tempFileName);
        
        try
        {
            // Generate the PDF to the temp file
            await GeneratePdfAsync(invoice, tempFilePath);
            
            // Open the PDF with the default viewer
            await OpenFileAsync(tempFilePath);
        }
        catch (Exception ex)
        {
            // Handle any exceptions
            Console.WriteLine($"Error previewing PDF: {ex.Message}");
            throw;
        }
    }
    
    private Task OpenFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("The generated PDF file was not found", filePath);
        
        try
        {
            // Open the file using the default application based on the OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", filePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", filePath);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported operating system");
            }
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error opening PDF file: {ex.Message}", ex);
        }
    }
}