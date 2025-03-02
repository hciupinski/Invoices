using System.ComponentModel.DataAnnotations;

namespace Invoices.Models;

public class InvoiceConfiguration
{
    // Company Information
    [Required]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
    public string CompanyAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
    public string CompanyCity { get; set; } = string.Empty;

    [Required]
    [StringLength(20, ErrorMessage = "Zip/Postal code cannot exceed 20 characters")]
    public string CompanyZipCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
    public string CompanyCountry { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "VAT/Tax ID cannot exceed 30 characters")]
    public string? CompanyTaxId { get; set; }

    [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
    public string? CompanyPhone { get; set; }

    [EmailAddress]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? CompanyEmail { get; set; }

    // Default tax rates
    [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100")]
    public decimal DefaultTaxRate { get; set; } = 0;

    // Bank details
    [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
    public string? BankName { get; set; }

    [StringLength(50, ErrorMessage = "Account number cannot exceed 50 characters")]
    public string? BankAccountNumber { get; set; }

    [StringLength(50, ErrorMessage = "IBAN cannot exceed 50 characters")]
    public string? BankIban { get; set; }

    [StringLength(20, ErrorMessage = "BIC/SWIFT cannot exceed 20 characters")]
    public string? BankSwift { get; set; }
    
    // Invoice storage path
    [StringLength(500, ErrorMessage = "Storage path cannot exceed 500 characters")]
    public string InvoicesStoragePath { get; set; } = string.Empty;
}