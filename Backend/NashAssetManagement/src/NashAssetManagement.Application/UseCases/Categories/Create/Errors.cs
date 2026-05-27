using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Categories.Create;

public static class CreateCategoryErrors
{
    public static Error CategoryAlreadyExists =>
        Error.Conflict(
            "Category.AlreadyExists",
            "Category is already existed. Please enter a different category.");

    public static Error PrefixAlreadyExists =>
        Error.Conflict(
            "Category.PrefixAlreadyExists",
            "Prefix is already existed. Please enter a different prefix.");
}