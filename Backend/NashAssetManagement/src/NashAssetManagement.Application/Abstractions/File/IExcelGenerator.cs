namespace NashAssetManagement.Application.Abstractions.File
{
    public interface IExcelGenerator<T>
    {
        byte[] Generate(IReadOnlyList<T> rows, string sheetName = "Sheet1");
    }
}