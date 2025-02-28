using System.ComponentModel.DataAnnotations;

namespace Invoices.Models;

public class InvoiceItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(0.01, 99999999.99, ErrorMessage = "Unit price must be greater than zero")]
    public double UnitPrice { get; set; }
    
    [Required]
    [Range(0.001, 99999.999, ErrorMessage = "Quantity must be greater than zero")]
    public double Quantity { get; set; } = 1;
    
    [Required]
    [StringLength(20, ErrorMessage = "Unit name cannot exceed 20 characters")]
    public string Unit { get; set; } = "pcs";
    
    [Required]
    [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100")]
    public decimal TaxRate { get; set; }
    
    public decimal NetValue => new decimal(UnitPrice * Quantity);
    
    public decimal TaxValue => NetValue * (TaxRate / 100);
    
    public decimal GrossValue => NetValue + TaxValue;
}