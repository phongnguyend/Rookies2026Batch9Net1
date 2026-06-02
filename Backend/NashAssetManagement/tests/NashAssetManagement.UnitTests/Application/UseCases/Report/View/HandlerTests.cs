using Ardalis.Specification;
using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Report.View;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Report.View
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<Category, Guid>> _mockCategoryRepo;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _mockCategoryRepo = new Mock<IRepository<Category, Guid>>();
            _mockValidator = new Mock<IValidator<Request>>();

            _handler = new Handler(
                _mockCategoryRepo.Object,
                _mockValidator.Object
            );
        }

        [Fact]
        [Trait("UT", "ViewReport")]
        public async Task Handle_ValidationFails_ShouldThrowValidationException()
        {
            // Arrange
            var request = new Request(10, 0, SortDirection.Asc, SortBy.Category);
            var failures = new List<ValidationFailure>
            {
                new(nameof(Request.PageNumber), "Page number must be greater than 0.")
            };

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            // Act
            var act = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ValidationException>(act);
        }

        [Fact]
        [Trait("UT", "ViewReport")]
        public async Task Handle_ValidRequest_ShouldReturnReportSuccessfully()
        {
            // Arrange
            var request = new Request(10, 1, SortDirection.Asc, SortBy.Category);
            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var categoryId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new()
                {
                    Id = categoryId,
                    CategoryName = "Laptop",
                    Prefix = "LA",
                    Assets = new List<Asset>
                    {
                        new() { Id = Guid.NewGuid(), CategoryId = categoryId, State = AssetState.Available },
                        new() { Id = Guid.NewGuid(), CategoryId = categoryId, State = AssetState.Assigned },
                        new() { Id = Guid.NewGuid(), CategoryId = categoryId, State = AssetState.Recycled }
                    }
                }
            };

            _mockCategoryRepo
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);

            var reportRow = result.Value.Items[0];
            Assert.Equal("Laptop", reportRow.CategoryName);
            Assert.Equal(3, reportRow.Total);
            Assert.Equal(1, reportRow.Available);
            Assert.Equal(1, reportRow.Assigned);
            Assert.Equal(1, reportRow.Recycled);
            Assert.Equal(0, reportRow.NotAvailable);
            Assert.Equal(0, reportRow.WaitingForRecycling);
        }
    }
}
