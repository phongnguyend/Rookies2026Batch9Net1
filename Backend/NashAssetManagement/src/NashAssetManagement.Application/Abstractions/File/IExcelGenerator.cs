namespace NashAssetManagement.Application.Abstractions.File
{
    public interface IExcelGenerator
    {
        byte[] Generate<T>(IReadOnlyList<T> rows, string sheetName = "Sheet1");
    }
}