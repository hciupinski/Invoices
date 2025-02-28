using System.ComponentModel.DataAnnotations;

namespace Invoices.Models;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime? CreationDate { get; set; } = DateTime.Now;
    
    [Required]
    public DateTime? SellDate { get; set; } = DateTime.Now;
    
    [Required]
    public DateTime? PaymentDate { get; set; } = DateTime.Now.AddDays(14);
    
    [Required]
    public string InvoiceNumber { get; set; } = string.Empty;
    
    // Contractor information - either selected from existing or entered manually
    public Guid? ContractorId { get; set; }
    
    [Required]
    public string ContractorName { get; set; } = string.Empty;
    
    [Required]
    public string ContractorAddress { get; set; } = string.Empty;
    
    [Required]
    public string ContractorCity { get; set; } = string.Empty;
    
    [Required]
    public string ContractorZipCode { get; set; } = string.Empty;
    
    [Required]
    public string ContractorCountry { get; set; } = string.Empty;
    
    public string? ContractorTaxId { get; set; }
    
    // Company information copied from configuration
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    public string CompanyAddress { get; set; } = string.Empty;
    
    [Required]
    public string CompanyCity { get; set; } = string.Empty;
    
    [Required]
    public string CompanyZipCode { get; set; } = string.Empty;
    
    [Required]
    public string CompanyCountry { get; set; } = string.Empty;
    
    public string? CompanyTaxId { get; set; }
    
    public string? CompanyPhone { get; set; }
    
    public string? CompanyEmail { get; set; }
    
    public string? BankName { get; set; }
    
    public string? BankAccountNumber { get; set; }
    
    public string? BankIban { get; set; }
    
    public string? BankSwift { get; set; }
    
    // Items
    public List<InvoiceItem> Items { get; set; } = new();
    
    // Totals
    public decimal TotalNetValue { get; set; }
    
    public decimal TotalTaxValue { get; set; }
    
    public decimal TotalGrossValue { get; set; }
    
    // Payment information
    public string? PaymentMethod { get; set; } = "Bank transfer";
    
    public string? Notes { get; set; }
    
    public bool IsPaid { get; set; }
    
    public DateTime? PaidDate { get; set; }
}