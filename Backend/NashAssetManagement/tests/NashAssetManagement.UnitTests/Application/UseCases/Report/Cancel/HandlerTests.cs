using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Report.Cancel;
using NashAssetManagement.Domain.Entities.Jobs.Report;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Report.Cancel
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<ExportReportJob, Guid>> _mockExportReportRepo;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<Handler>> _mockLogger;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _mockExportReportRepo = new Mock<IRepository<ExportReportJob, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<Handler>>();

            _handler = new Handler(
                _mockExportReportRepo.Object,
                _mockUser.Object,
                _mockUow.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        [Trait("UT", "CancelReport")]
        public async Task Handle_ActiveReportExists_ShouldDeleteJobAndFile()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Create a physical temp file to test File.Delete
            var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "TempReportFolders");
            Directory.CreateDirectory(tempFolder);
            var tempFile = Path.Combine(tempFolder, $"{Guid.NewGuid()}_test_cancel_report.xlsx");
            await File.WriteAllTextAsync(tempFile, "mock excel content");
            var relativeFilePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), tempFile);

            var existingJob = new ExportReportJob
            {
                Id = Guid.NewGuid(),
                RequestedByAdminId = userId,
                Status = ExportReportJobStatus.ReadyToDownload,
                FilePath = relativeFilePath,
                CreatedAtUtc = DateTime.UtcNow
            };

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);

            _mockExportReportRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ExportReportJob>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            try
            {
                // Act
                var result = await _handler.Handle(new Request(), CancellationToken.None);

                // Assert
                Assert.False(result.IsError);
                Assert.False(File.Exists(tempFile)); // Verify file was physically deleted

                _mockExportReportRepo.Verify(r => r.Delete(existingJob), Times.Once);
                _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            }
            finally
            {
                // Cleanup file if test failed to delete it
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        [Trait("UT", "CancelReport")]
        public async Task Handle_NoActiveReport_ShouldReturnReportNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);

            _mockExportReportRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ExportReportJob>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ExportReportJob?)null);

            // Act
            var result = await _handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ReportNotFound, result.FirstError);

            _mockExportReportRepo.Verify(r => r.Delete(It.IsAny<ExportReportJob>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
