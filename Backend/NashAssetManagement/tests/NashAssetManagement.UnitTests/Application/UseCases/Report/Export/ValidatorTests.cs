using NashAssetManagement.Application.UseCases.Report.Export;
using NashAssetManagement.Domain.Entities.Jobs.Report;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Report.Export
{
    public class ValidatorTests
    {
        private readonly Validator _validator = new();

        [Fact]
        [Trait("UT", "ExportReport")]
        public async Task Validate_RequestIsValid_Pass()
        {
            // Arrange
            var request = new Request(ExportReportSortDirection.Asc, ExportReportSortBy.Category);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        [Trait("UT", "ExportReport")]
        public async Task Validate_SortByOrSortDirectionIsInvalid_ReturnError()
        {
            // Arrange
            var request = new Request((ExportReportSortDirection)999, (ExportReportSortBy)999);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "SortBy" && x.ErrorMessage == Validator.SortByMustBeValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "SortDirection" && x.ErrorMessage == Validator.SortDirectionMustBeValid);
        }
    }
}
