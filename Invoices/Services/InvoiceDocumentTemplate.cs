using System.Globalization;
using Invoices.Extensions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Invoices.Models;
using LiczbyNaSlowaNETCore;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace Invoices.Services;

public class InvoiceDocumentTemplate : IDocument
{
    private readonly Invoice _invoice;
    private CultureInfo _culture;

    public InvoiceDocumentTemplate(Invoice invoice)
    {
        _invoice = invoice;
        _culture = new CultureInfo("pl-PL");
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
                column.Item().Text($"Faktura VAT {_invoice.InvoiceNumber}")
                    .FontSize(18)
                    .SemiBold();

                column.Item().Text($"Data sprzedaży: {_invoice.SellDate:yyyy-MM-dd}");
                column.Item().Text($"Data wystawienia: {_invoice.CreationDate:yyyy-MM-dd}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().PaddingVertical(20).Column(cd =>
                    {
                        cd.Item().Text("Sprzedawca:").SemiBold();
                        cd.Item().Text(_invoice.CompanyName);
                        cd.Item().Text(_invoice.CompanyAddress);
                        cd.Item().Text($"{_invoice.CompanyZipCode} {_invoice.CompanyCity}");
                        cd.Item().Text(_invoice.CompanyCountry);
                        cd.Item().Text($"NIP: {_invoice.CompanyTaxId}");
                    });
                });

                row.RelativeItem().Column(c =>
                {
                    c.Item().PaddingVertical(20).Column(cd =>
                    {
                        cd.Item().Text("Nabywca:").SemiBold();
                        cd.Item().Text(_invoice.ContractorName);
                        cd.Item().Text(_invoice.ContractorAddress);
                        cd.Item().Text($"{_invoice.ContractorZipCode} {_invoice.ContractorCity}");
                        cd.Item().Text(_invoice.ContractorCountry);
                        cd.Item().Text($"NIP: {_invoice.ContractorTaxId}");
                    });
                });
            });

            column.Item().Column(c =>
            {
                // Items table
                c.Item().PaddingVertical(20).Table(table =>
                {
                    // Define columns
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4); // Description
                        columns.RelativeColumn(1); // Quantity
                        columns.RelativeColumn(1.5f); // Unit
                        columns.RelativeColumn(1.5f); // Unit Price
                        columns.RelativeColumn(1.5f); // Tax Rate
                        columns.RelativeColumn(1.5f); // Net Value
                        columns.RelativeColumn(1.5f); // Tax Value
                        columns.RelativeColumn(1.5f); // Gross Value
                    });

                    // Table header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Opis").FontSize(10).SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Ilość").FontSize(10).SemiBold()
                            .AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Jedn.").FontSize(10)
                            .SemiBold()
                            .AlignCenter();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Cena jedn.").FontSize(10)
                            .SemiBold()
                            .AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Stawka VAT").FontSize(10)
                            .SemiBold()
                            .AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Netto").FontSize(10).SemiBold()
                            .AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("VAT").FontSize(10).SemiBold()
                            .AlignRight();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(2).Text("Brutto").FontSize(10).SemiBold()
                            .AlignRight();
                    });

                    // Table items
                    foreach (var item in _invoice.Items)
                    {
                        table.Cell().Padding(2).Text(item.Description).FontSize(10);
                        table.Cell().Padding(2).Text(item.Quantity.ToString("0.###")).FontSize(10).AlignRight();
                        table.Cell().Padding(2).Text(item.Unit).FontSize(10).AlignCenter();
                        table.Cell().Padding(2).Text(item.UnitPrice.ToCurrencyString()).FontSize(10).AlignRight();
                        table.Cell().Padding(2).Text($"{item.TaxRate}%").FontSize(10).AlignRight();
                        table.Cell().Padding(2).Text(item.NetValue.ToCurrencyString()).FontSize(10).AlignRight();
                        table.Cell().Padding(2).Text(item.TaxValue.ToCurrencyString()).FontSize(10).AlignRight();
                        table.Cell().Padding(2).Text(item.GrossValue.ToCurrencyString()).FontSize(10).AlignRight();
                    }

                    // Table footer with totals
                    table.Cell().ColumnSpan(5).PaddingTop(5).Text("Suma:").FontSize(10).SemiBold().AlignRight();
                    table.Cell().PaddingTop(5).Text(_invoice.TotalNetValue.ToCurrencyString()).FontSize(10).SemiBold()
                        .AlignRight();
                    table.Cell().PaddingTop(5).Text(_invoice.TotalTaxValue.ToCurrencyString()).FontSize(10).SemiBold()
                        .AlignRight();
                    table.Cell().PaddingTop(5).Text(_invoice.TotalGrossValue.ToCurrencyString()).FontSize(10).SemiBold()
                        .AlignRight();
                });
                
                c.Item().PaddingVertical(20).Column(cd =>
                {
                    cd.Item().Text("Do zapłaty:").FontSize(10).SemiBold();
                    cd.Item().PaddingVertical(5).Text(_invoice.TotalGrossValue.ToCurrencyString()).SemiBold();
                    cd.Item().Text("Słownie do zapłaty:").FontSize(10).SemiBold();
                    
                    var options = new NumberToTextOptions
                    {
                        SplitDecimal = "i",
                        Currency = Currency.PLN,
                        Stems = true
                    };
                    
                    cd.Item().PaddingVertical(5).Text(NumberToText.Convert(_invoice.TotalGrossValue, options)).SemiBold();
                });

                // Payment information
                c.Item().PaddingVertical(20).Column(cd =>
                {
                    cd.Item().PaddingVertical(5).Text("Szczegóły płatności:").FontSize(10).SemiBold();

                    cd.Item().Text($"Metoda płatności: {_invoice.PaymentMethod}");
                    cd.Item().Text($"Bank: {_invoice.BankName}");
                    cd.Item().Text($"Numer konta: {_invoice.BankAccountNumber}");
                    cd.Item().Text($"Termin płatności: {_invoice.PaymentDate:yyyy-MM-dd}");
                });


                // Notes
                c.Item().PaddingVertical(10).Column(cd =>
                {
                    cd.Item().Text("Opis:").FontSize(10).SemiBold();
                    if (_invoice.Notes != null && _invoice.Notes.Any())
                    {
                        cd.Item().Text(_invoice.Notes);
                    }
                    else
                    {
                        cd.Item().Text("brak");
                    }

                });

                c.Item().AlignRight().PaddingTop(40).Column(cd =>
                {
                    cd.Item().Text(_invoice.CompanyName.PadLeft(5, ' ').PadRight(5, ' ')).Underline().DecorationDotted().SemiBold();
                    cd.Item().AlignCenter().Text("Podpis sprzedawcy").FontSize(8).SemiBold();
                });
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().AlignCenter().Text(text =>
            {
                text.Span("Strona  ");
                text.CurrentPageNumber();
                text.Span(" z ");
                text.TotalPages();
            });
        });
    }
}