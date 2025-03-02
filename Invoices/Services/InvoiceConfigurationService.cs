using System.Text.Json;
using Invoices.Models;

namespace Invoices.Services;

public class InvoiceConfigurationService
{
    private readonly string _configFilePath;
    private readonly string _contractorsFilePath;
    private InvoiceConfiguration _configuration = new();
    private List<Contractor> _contractors = new();

    public InvoiceConfigurationService()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Invoices");
        Directory.CreateDirectory(appDataPath);
        
        _configFilePath = Path.Combine(appDataPath, "config.json");
        _contractorsFilePath = Path.Combine(appDataPath, "contractors.json");
        
        LoadConfiguration();
        LoadContractors();
        
        // Set default invoices storage path if it's not set
        if (string.IsNullOrEmpty(_configuration.InvoicesStoragePath))
        {
            _configuration.InvoicesStoragePath = GetDefaultInvoicesPath();
            SaveConfigurationAsync(_configuration).GetAwaiter().GetResult();
        }
    }
    
    private string GetDefaultInvoicesPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "Invoices"
        );
    }

    public InvoiceConfiguration GetConfiguration() => _configuration;

    public async Task SaveConfigurationAsync(InvoiceConfiguration configuration)
    {
        _configuration = configuration;
        var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_configFilePath, json);
    }

    public IEnumerable<Contractor> GetContractors() => _contractors.Where(c => c.IsActive).ToList();

    public Contractor? GetContractor(Guid id) => _contractors.FirstOrDefault(c => c.Id == id);

    public async Task SaveContractorAsync(Contractor contractor)
    {
        var existingIndex = _contractors.FindIndex(c => c.Id == contractor.Id);
        
        if (existingIndex >= 0)
        {
            _contractors[existingIndex] = contractor;
        }
        else
        {
            _contractors.Add(contractor);
        }
        
        await SaveContractorsAsync();
    }

    public async Task DeleteContractorAsync(Guid id)
    {
        var contractor = _contractors.FirstOrDefault(c => c.Id == id);
        if (contractor != null)
        {
            contractor.IsActive = false;
            await SaveContractorsAsync();
        }
    }

    private void LoadConfiguration()
    {
        if (File.Exists(_configFilePath))
        {
            try
            {
                var json = File.ReadAllText(_configFilePath);
                _configuration = JsonSerializer.Deserialize<InvoiceConfiguration>(json) ?? new InvoiceConfiguration();
            }
            catch
            {
                _configuration = new InvoiceConfiguration();
            }
        }
    }

    private void LoadContractors()
    {
        if (File.Exists(_contractorsFilePath))
        {
            try
            {
                var json = File.ReadAllText(_contractorsFilePath);
                _contractors = JsonSerializer.Deserialize<List<Contractor>>(json) ?? new List<Contractor>();
            }
            catch
            {
                _contractors = new List<Contractor>();
            }
        }
    }

    private async Task SaveContractorsAsync()
    {
        var json = JsonSerializer.Serialize(_contractors, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_contractorsFilePath, json);
    }
}