using System.ComponentModel.DataAnnotations;

namespace Invoices.Models;

public class Contractor
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(20, ErrorMessage = "Zip/Postal code cannot exceed 20 characters")]
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
    public string Country { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "VAT/Tax ID cannot exceed 30 characters")]
    public string? TaxId { get; set; }

    [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;
}