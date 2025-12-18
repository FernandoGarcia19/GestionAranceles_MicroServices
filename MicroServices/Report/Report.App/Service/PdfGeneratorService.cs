using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Report.Dom.Model;

namespace Report.App.Service;

public class PdfGeneratorService
{
    public byte[] GeneratePaymentPdf(PaymentReport payment)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text("COMPROBANTE DE PAGO")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(20);

                        // Payment Information Section
                        column.Item().Text("Información del Pago").SemiBold().FontSize(14);
                        
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(150);
                                columns.RelativeColumn();
                            });

                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Establecimiento").SemiBold();
                            table.Cell().Border(1).Padding(5).Text(payment.EstablishmentName);

                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Fecha de Pago").SemiBold();
                            table.Cell().Border(1).Padding(5).Text(payment.PaymentDate.ToString("dd/MM/yyyy"));

                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Método de Pago").SemiBold();
                            table.Cell().Border(1).Padding(5).Text(payment.PaymentMethod);

                            table.Cell().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Número de Recibo").SemiBold();
                            table.Cell().Border(1).Padding(5).Text($"#{payment.ReceiptNumber}");
                        });

                        // Categories Section
                        column.Item().PaddingTop(20).Text("Categorías de Pago").SemiBold().FontSize(14);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Categoría").FontColor(Colors.White).SemiBold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Cantidad").FontColor(Colors.White).SemiBold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Precio Unitario").FontColor(Colors.White).SemiBold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Subtotal").FontColor(Colors.White).SemiBold();
                            });

                            // Rows
                            foreach (var item in payment.Items)
                            {
                                table.Cell().Border(1).Padding(5).Text(item.CategoryName);
                                table.Cell().Border(1).Padding(5).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Border(1).Padding(5).AlignRight().Text($"{item.UnitPrice:F2} Bs");
                                table.Cell().Border(1).Padding(5).AlignRight().Text($"{item.Subtotal:F2} Bs");
                            }
                        });

                        // Total Section
                        column.Item().PaddingTop(10).AlignRight().Row(row =>
                        {
                            row.AutoItem().Background(Colors.Green.Lighten3).Padding(10).Column(col =>
                            {
                                col.Item().Text("MONTO TOTAL PAGADO").SemiBold().FontSize(12);
                                col.Item().Text($"{payment.AmountPaid:F2} Bs").Bold().FontSize(16).FontColor(Colors.Green.Darken2);
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Column(col =>
                    {
                        col.Item().Text(x =>
                        {
                            x.Span("Generado el: ");
                            x.Span(payment.GeneratedDate.ToString("dd/MM/yyyy HH:mm:ss")).SemiBold();
                        });
                        
                        if (!string.IsNullOrEmpty(payment.GeneratedBy))
                        {
                            col.Item().Text(x =>
                            {
                                x.Span("Por: ");
                                x.Span(payment.GeneratedBy).SemiBold();
                            });
                        }
                    });
            });
        });

        return document.GeneratePdf();
    }
}
