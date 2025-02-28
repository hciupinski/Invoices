using System.Text.Json;
using Invoices.Models;

namespace Invoices.Services;

public class InvoiceService
{
    private readonly string _invoicesFilePath;
    private readonly InvoiceConfigurationService _configService;
    private List<Invoice> _invoices = new();
    
    public InvoiceService(InvoiceConfigurationService configService)
    {
        _configService = configService;
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Invoices");
        Directory.CreateDirectory(appDataPath);
        
        _invoicesFilePath = Path.Combine(appDataPath, "invoices.json");
        LoadInvoices();
    }
    
    public IEnumerable<Invoice> GetInvoices() => _invoices.OrderByDescending(i => i.CreationDate).ToList();
    
    public Invoice? GetInvoice(Guid id) => _invoices.FirstOrDefault(i => i.Id == id);
    
    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
    {
        // Generate invoice number if not provided
        if (string.IsNullOrEmpty(invoice.InvoiceNumber))
        {
            invoice.InvoiceNumber = GenerateInvoiceNumber();
        }
        
        // Calculate totals
        CalculateInvoiceTotals(invoice);
        
        _invoices.Add(invoice);
        await SaveInvoicesAsync();
        
        return invoice;
    }
    
    public async Task DeleteInvoiceAsync(Guid id)
    {
        var invoice = _invoices.FirstOrDefault(i => i.Id == id);
        if (invoice != null)
        {
            _invoices.Remove(invoice);
            await SaveInvoicesAsync();
        }
    }
    
    public Invoice CreateNewInvoice()
    {
        var config = _configService.GetConfiguration();
        
        var invoice = new Invoice
        {
            // Copy company details from configuration
            CompanyName = config.CompanyName,
            CompanyAddress = config.CompanyAddress,
            CompanyCity = config.CompanyCity,
            CompanyZipCode = config.CompanyZipCode,
            CompanyCountry = config.CompanyCountry,
            CompanyTaxId = config.CompanyTaxId,
            CompanyPhone = config.CompanyPhone,
            CompanyEmail = config.CompanyEmail,
            BankName = config.BankName,
            BankAccountNumber = config.BankAccountNumber,
            BankIban = config.BankIban,
            BankSwift = config.BankSwift,
            
            // Set default tax rate
            Items = new List<InvoiceItem>
            {
                new() { TaxRate = config.DefaultTaxRate }
            }
        };
        
        return invoice;
    }
    
    private void CalculateInvoiceTotals(Invoice invoice)
    {
        invoice.TotalNetValue = invoice.Items.Sum(i => i.NetValue);
        invoice.TotalTaxValue = invoice.Items.Sum(i => i.TaxValue);
        invoice.TotalGrossValue = invoice.Items.Sum(i => i.GrossValue);
    }
    
    private string GenerateInvoiceNumber()
    {
        // Format: INV/YYYYMMDD/XX where XX is a sequential number
        var date = DateTime.Now.ToString("yyyyMMdd");
        var dailyInvoices = _invoices.Count(i => i.InvoiceNumber.Contains(date));
        
        return $"INV/{date}/{(dailyInvoices + 1):D2}";
    }
    
    private void LoadInvoices()
    {
        if (File.Exists(_invoicesFilePath))
        {
            try
            {
                var json = File.ReadAllText(_invoicesFilePath);
                _invoices = JsonSerializer.Deserialize<List<Invoice>>(json) ?? new List<Invoice>();
            }
            catch
            {
                _invoices = new List<Invoice>();
            }
        }
    }
    
    private async Task SaveInvoicesAsync()
    {
        var json = JsonSerializer.Serialize(_invoices, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_invoicesFilePath, json);
    }
}