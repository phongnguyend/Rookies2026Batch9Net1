using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.UseCases.Report.CurrentDownload;
using NashAssetManagement.Domain.Entities.Jobs.Report;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Report.CurrentDownload
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<ExportReportJob, Guid>> _mockExportReportRepo;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<ILogger<Handler>> _mockLogger;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _mockExportReportRepo = new Mock<IRepository<ExportReportJob, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockLogger = new Mock<ILogger<Handler>>();

            _handler = new Handler(
                _mockExportReportRepo.Object,
                _mockUser.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        [Trait("UT", "CurrentDownload")]
        public async Task Handle_NoActiveReport_ReturnResponseWithNullStatus()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
            _mockUser.Setup(u => u.Username).Returns("admin");

            _mockExportReportRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ExportReportJob>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ExportReportJob?)null);

            // Act
            var result = await _handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Null(result.Value.Status);
            Assert.Null(result.Value.DownloadUrl);
        }

        [Fact]
        [Trait("UT", "CurrentDownload")]
        public async Task Handle_ActiveReportExists_ReturnResponseWithJobStatus()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingJob = new ExportReportJob
            {
                Id = Guid.NewGuid(),
                RequestedByAdminId = userId,
                Status = ExportReportJobStatus.ReadyToDownload,
                FilePath = "TempReportFolders\\2026-06-04_HCM_admin_report.xlsx",
                CreatedAtUtc = new DateTime(2026, 6, 4, 12, 0, 0, DateTimeKind.Utc)
            };

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
            _mockUser.Setup(u => u.Username).Returns("admin");

            _mockExportReportRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ExportReportJob>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            // Act
            var result = await _handler.Handle(new Request(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(ExportReportJobStatus.ReadyToDownload, result.Value.Status);
            Assert.Equal("/TempReportFolders/2026-06-04_HCM_admin_report.xlsx", result.Value.DownloadUrl);
        }
    }
}
