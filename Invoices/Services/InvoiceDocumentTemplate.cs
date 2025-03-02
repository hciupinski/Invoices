using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Invoices.Models;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace Invoices.Services;

public class InvoiceDocumentTemplate : IDocument
{
    private readonly Invoice _invoice;

    public InvoiceDocumentTemplate(Invoice invoice)
    {
        _invoice = invoice;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);
                
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            // Invoice details (left side)
            row.RelativeItem().Column(column =>
            {
                column.Item().Text($"INVOICE #{_invoice.InvoiceNumber}")
                    .FontSize(20)
                    .SemiBold();
                
                column.Item().Text($"Issue Date: {_invoice.CreationDate:yyyy-MM-dd}");
                column.Item().Text($"Due Date: {_invoice.PaymentDate:yyyy-MM-dd}");
                
                if (_invoice.IsPaid)
                {
                    column.Item().AlignCenter().Background(Colors.Green.Lighten3)
                        .Padding(5).Text("PAID")
                        .FontColor(Colors.Green.Darken3);
                }
            });
            
            // Company details (right side)
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(_invoice.CompanyName).SemiBold();
                column.Item().Text(_invoice.CompanyAddress);
                column.Item().Text($"{_invoice.CompanyZipCode} {_invoice.CompanyCity}");
                column.Item().Text(_invoice.CompanyCountry);
                
                if (!string.IsNullOrEmpty(_invoice.CompanyTaxId))
                    column.Item().Text($"Tax ID: {_invoice.CompanyTaxId}");
                
                if (!string.IsNullOrEmpty(_invoice.CompanyPhone))
                    column.Item().Text($"Phone: {_invoice.CompanyPhone}");
                
                if (!string.IsNullOrEmpty(_invoice.CompanyEmail))
                    column.Item().Text($"Email: {_invoice.CompanyEmail}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            // Contractor information
            column.Item().PaddingVertical(10).Column(c =>
            {
                c.Item().Text("Bill To:").SemiBold();
                c.Item().Text(_invoice.ContractorName);
                c.Item().Text(_invoice.ContractorAddress);
                c.Item().Text($"{_invoice.ContractorZipCode} {_invoice.ContractorCity}");
                c.Item().Text(_invoice.ContractorCountry);
                
                if (!string.IsNullOrEmpty(_invoice.ContractorTaxId))
                    c.Item().Text($"Tax ID: {_invoice.ContractorTaxId}");
            });
            
            // Items table
            column.Item().PaddingVertical(10).Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);    // Description
                    columns.RelativeColumn(1);    // Quantity
                    columns.RelativeColumn(1);    // Unit
                    columns.RelativeColumn(1.5f); // Unit Price
                    columns.RelativeColumn(1);    // Tax Rate
                    columns.RelativeColumn(1.5f); // Net Value
                    columns.RelativeColumn(1.5f); // Tax Value
                    columns.RelativeColumn(1.5f); // Gross Value
                });
                
                // Table header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Description").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Qty").SemiBold().AlignRight();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Unit").SemiBold().AlignCenter();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Unit Price").SemiBold().AlignRight();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Tax %").SemiBold().AlignRight();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Net").SemiBold().AlignRight();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Tax").SemiBold().AlignRight();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Gross").SemiBold().AlignRight();
                });
                
                // Table items
                foreach (var item in _invoice.Items)
                {
                    table.Cell().Padding(2).Text(item.Description);
                    table.Cell().Padding(2).Text(item.Quantity.ToString("0.###")).AlignRight();
                    table.Cell().Padding(2).Text(item.Unit).AlignCenter();
                    table.Cell().Padding(2).Text($"{item.UnitPrice:C2}").AlignRight();
                    table.Cell().Padding(2).Text($"{item.TaxRate}%").AlignRight();
                    table.Cell().Padding(2).Text($"{item.NetValue:C2}").AlignRight();
                    table.Cell().Padding(2).Text($"{item.TaxValue:C2}").AlignRight();
                    table.Cell().Padding(2).Text($"{item.GrossValue:C2}").AlignRight();
                }
                
                // Table footer with totals
                table.Cell().ColumnSpan(5).PaddingTop(5).Text("Total:").SemiBold().AlignRight();
                table.Cell().PaddingTop(5).Text($"{_invoice.TotalNetValue:C2}").SemiBold().AlignRight();
                table.Cell().PaddingTop(5).Text($"{_invoice.TotalTaxValue:C2}").SemiBold().AlignRight();
                table.Cell().PaddingTop(5).Text($"{_invoice.TotalGrossValue:C2}").SemiBold().AlignRight();
            });

            // Payment information
            if (!string.IsNullOrEmpty(_invoice.PaymentMethod) || 
                !string.IsNullOrEmpty(_invoice.BankName) || 
                !string.IsNullOrEmpty(_invoice.BankAccountNumber))
            {
                column.Item().PaddingVertical(10).Column(c =>
                {
                    c.Item().Text("Payment Details:").SemiBold();
                    
                    if (!string.IsNullOrEmpty(_invoice.PaymentMethod))
                        c.Item().Text($"Payment Method: {_invoice.PaymentMethod}");
                    
                    if (!string.IsNullOrEmpty(_invoice.BankName))
                        c.Item().Text($"Bank: {_invoice.BankName}");
                    
                    if (!string.IsNullOrEmpty(_invoice.BankAccountNumber))
                        c.Item().Text($"Account Number: {_invoice.BankAccountNumber}");
                    
                    if (!string.IsNullOrEmpty(_invoice.BankIban))
                        c.Item().Text($"IBAN: {_invoice.BankIban}");
                    
                    if (!string.IsNullOrEmpty(_invoice.BankSwift))
                        c.Item().Text($"SWIFT: {_invoice.BankSwift}");
                });
            }
            
            // Notes
            if (!string.IsNullOrEmpty(_invoice.Notes))
            {
                column.Item().PaddingVertical(10).Column(c =>
                {
                    c.Item().Text("Notes:").SemiBold();
                    c.Item().Text(_invoice.Notes);
                });
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().AlignCenter().Text(text =>
            {
                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
            });
        });
    }
}