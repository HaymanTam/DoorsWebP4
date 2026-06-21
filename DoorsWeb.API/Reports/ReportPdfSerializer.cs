using DoorsWeb.Shared.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DoorsWeb.API.Reports
{
    /// <summary>Renders a <see cref="ReportResult"/> to a landscape A4 PDF table via QuestPDF.</summary>
    public static class ReportPdfSerializer
    {
        public static byte[] ToPdf(ReportResult result)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(28);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Text(result.Title).FontSize(16).SemiBold();
                        col.Item().Text($"Generated {result.GeneratedAtUtc:yyyy-MM-dd HH:mm} UTC · {result.RowCount} row(s)")
                            .FontSize(8).FontColor(Colors.Grey.Darken1);
                        if (result.AppliedParameters.Count > 0)
                            col.Item().Text(string.Join("    ", result.AppliedParameters))
                                .FontSize(8).FontColor(Colors.Grey.Darken1);
                        if (result.Truncated)
                            col.Item().Text("Result truncated — refine the parameters to see the remaining rows.")
                                .FontSize(8).FontColor(Colors.Red.Medium);
                    });

                    page.Content().PaddingVertical(8).Element(content =>
                    {
                        if (result.Columns.Count == 0)
                        {
                            content.Text("No columns to display.").FontColor(Colors.Grey.Medium);
                            return;
                        }

                        content.Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var _ in result.Columns)
                                    columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                foreach (var c in result.Columns)
                                    Align(header.Cell().Element(HeaderCellStyle), c.Align)
                                        .Text(c.Label).SemiBold();
                            });

                            foreach (var row in result.Rows)
                            {
                                for (var i = 0; i < result.Columns.Count; i++)
                                {
                                    var text = i < row.Count ? row[i] : null;
                                    Align(table.Cell().Element(BodyCellStyle), result.Columns[i].Align)
                                        .Text(text ?? string.Empty);
                                }
                            }
                        });
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private static IContainer Align(IContainer cell, ColumnAlign align) => align switch
        {
            ColumnAlign.Right => cell.AlignRight(),
            ColumnAlign.Center => cell.AlignCenter(),
            _ => cell.AlignLeft()
        };

        private static IContainer HeaderCellStyle(IContainer c) =>
            c.Background(Colors.Grey.Lighten3)
             .BorderBottom(1).BorderColor(Colors.Grey.Medium)
             .PaddingVertical(4).PaddingHorizontal(3);

        private static IContainer BodyCellStyle(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
             .PaddingVertical(3).PaddingHorizontal(3);
    }
}
