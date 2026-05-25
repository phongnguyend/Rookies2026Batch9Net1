using Ardalis.Specification;
using Moq;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Assignments.GetById;
using NashAssetManagement.Domain.Entities.Core;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Assignments
{
    public class GetByIdHandlerTest
    {
        private static Mock<IRepository<Assignment, Guid>> CreateRepoMock() =>
            new Mock<IRepository<Assignment, Guid>>();

        [Fact]
        public async Task Handle_Should_Return_AssignmentNotFound_When_Assignment_Does_Not_Exist()
        {
            // Arrange
            var repoMock = CreateRepoMock();

            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response  >>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Response?)null);

            var handler = new Handler(repoMock.Object);

            var query = new Query(Guid.NewGuid());

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("Assignment.AssignmentNotFoundWithId", result.FirstError.Code);
        }

        [Fact]
        public async Task Handle_Should_Return_Response_When_Assignment_Exists()
        {
            // Arrange
            var repoMock = CreateRepoMock();

            var assignmentId = Guid.NewGuid();
            var response = new Response(
                assignmentId,
                "AS001",
                "Laptop Dell XPS",
                "Core i7, 16GB RAM",
                "userA",
                "admin",
                "01/01/2024",
                "Accepted",
                "Handle with care");

            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var handler = new Handler(repoMock.Object);

            var query = new Query(assignmentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(assignmentId, result.Value.Id);
            Assert.Equal("AS001", result.Value.AssetCode);
            Assert.Equal("Laptop Dell XPS", result.Value.AssetName);
            Assert.Equal("Core i7, 16GB RAM", result.Value.Specification);
            Assert.Equal("userA", result.Value.AssignedTo);
            Assert.Equal("admin", result.Value.AssignedBy);
            Assert.Equal("01/01/2024", result.Value.AssignedDate);
            Assert.Equal("Accepted", result.Value.State);
            Assert.Equal("Handle with care", result.Value.Note);
        }

        [Fact]
        public async Task Handle_Should_Return_AssignmentNotFound_With_Correct_Id_In_Description()
        {
            // Arrange
            var repoMock = CreateRepoMock();
            var assignmentId = Guid.NewGuid();

            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Response?)null);

            var handler = new Handler(repoMock.Object);

            var query = new Query(assignmentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Contains(assignmentId.ToString(), result.FirstError.Description);
        }

        [Fact]
        public async Task Handle_Should_Return_Response_With_Empty_Strings_For_Optional_Fields()
        {
            // Arrange
            var repoMock = CreateRepoMock();

            var assignmentId = Guid.NewGuid();
            var response = new Response(
                assignmentId,
                "AS002",
                "Mouse Logitech",
                "",         // Specification is null → ""
                "userB",
                "admin",
                "15/06/2024",
                "Waiting",
                "");        // Note is null → ""

            repoMock
                .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Assignment, Response>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var handler = new Handler(repoMock.Object);

            var query = new Query(assignmentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("", result.Value.Specification);
            Assert.Equal("", result.Value.Note);
        }
    }
}
