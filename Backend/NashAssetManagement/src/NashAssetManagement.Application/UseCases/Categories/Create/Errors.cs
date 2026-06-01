using ErrorOr;

namespace NashAssetManagement.Application.UseCases.Categories.Create;

public static class CreateCategoryErrors
{
    public static readonly Error CategoryAlreadyExists = Error.Conflict(
        code: "CategoryCreate.CategoryAlreadyExists",
        description: "Category is already existed. Please enter a different category."
    );

    public static readonly Error PrefixAlreadyExists = Error.Conflict(
        code: "CategoryCreate.PrefixAlreadyExists",
        description: "Prefix is already existed. Please enter a different prefix."
    );

    public static readonly Error CategoryPrefixRequired = Error.Validation(
        code: "CategoryCreate.CategoryPrefixRequired",
        description: "Category prefix is required when creating a new category."
    );

    public static readonly Error CategoryCreationFailed = Error.Failure(
        code: "CategoryCreate.CreationFailed",
        description: "Failed to create category. Please try again."
    );
}