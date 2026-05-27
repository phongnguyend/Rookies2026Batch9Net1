using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Assets.Create;

public static class CreateAssetErrors
{
    public static Error CategoryAlreadyExists =>
        Error.Conflict(
            "Asset.CategoryAlreadyExists",
            "Category is already existed. Please enter a different category.");

    public static Error PrefixAlreadyExists =>
        Error.Conflict(
            "Asset.PrefixAlreadyExists",
            "Prefix is already existed. Please enter a different prefix.");

    public static Error CategoryPrefixRequired =>
        Error.Validation(
            "Asset.CategoryPrefixRequired",
            "Category prefix is required when creating a new category.");
}