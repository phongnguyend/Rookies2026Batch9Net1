using System.Reflection;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.File;
using NashAssetManagement.Application.Abstractions.Report;

namespace NashAssetManagement.Infrastructure.Report
{
    public sealed class AssetReportExcelGenerator(
        ILogger<AssetReportExcelGenerator> logger
    ) : IExcelGenerator<AssetReportRow>
    {
        public byte[] Generate(IReadOnlyList<AssetReportRow> rows, string sheetName = "Sheet1")
        {
            if (rows == null || rows.Count == 0 || string.IsNullOrEmpty(sheetName))
            {
                logger.LogWarning("No data to generate report.");
                throw new InvalidOperationException("Cannot generate report with empty data.");
            }

            // workbook
            using var workbook = new XLWorkbook();

            // new sheet
            var sheet = workbook.Worksheets.Add(sheetName);

            // excel's properties
            var properties = typeof(AssetReportRow).GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            );

            // headers
            for (int col = 0; col < properties.Length; col++)
            {
                sheet.Cell(1, col + 1).Value = properties[col].Name;
            }

            var headerRange = sheet.Range(1, 1, 1, properties.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.RedPigment;

            // data rows
            for (int row = 0; row < rows.Count; row++)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(rows[row]);
                    var cell = sheet.Cell(row + 2, col + 1);
                    cell.Value = value?.ToString() ?? string.Empty;
                }
            }

            // improvement for sheet
            sheet.Columns().AdjustToContents();
            sheet.SheetView.FreezeRows(1);

            // save to stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}