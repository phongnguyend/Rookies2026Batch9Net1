using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Application.Abstractions.DateTimes;
using NashAssetManagement.Application.UseCases.Report.Export;
using NashAssetManagement.Domain.Entities.Jobs.Report;
using NashAssetManagement.Domain.Enums;
using Xunit;

namespace NashAssetManagement.UnitTests.Application.UseCases.Report.Export
{
    public class HandlerTests
    {
        private readonly Mock<IRepository<ExportReportJob, Guid>> _mockExportReportRepo;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<Handler>> _mockLogger;
        private readonly Mock<IValidator<Request>> _mockValidator;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
        private readonly Handler _handler;

        public HandlerTests()
        {
            _mockExportReportRepo = new Mock<IRepository<ExportReportJob, Guid>>();
            _mockUser = new Mock<ICurrentUser>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<Handler>>();
            _mockValidator = new Mock<IValidator<Request>>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();

            _mockDateTimeProvider.Setup(d => d.UtcNow).Returns(DateTime.UtcNow);

            _handler = new Handler(
                _mockExportReportRepo.Object,
                _mockUser.Object,
                _mockUow.Object,
                _mockLogger.Object,
                _mockValidator.Object,
                _mockBackgroundJobClient.Object,
                _mockDateTimeProvider.Object
            );
        }

        [Fact]
        [Trait("UT", "ExportReport")]
        public async Task Handle_ValidRequest_CreateJobAndEnqueueBackgroundJob()
        {
            // Arrange
            var request = new Request(ExportReportSortDirection.Asc, ExportReportSortBy.Category);
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();
            var username = "admin";

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
            _mockUser.Setup(u => u.LocationId).Returns(locationId.ToString());
            _mockUser.Setup(u => u.Username).Returns(username);

            _mockExportReportRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ExportReportJob>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ExportReportJob?)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(ExportReportJobStatus.Processing, result.Value.Status);

            _mockExportReportRepo.Verify(r => r.AddAsync(It.IsAny<ExportReportJob>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockBackgroundJobClient.Verify(
                x => x.Create(It.IsAny<Hangfire.Common.Job>(), It.IsAny<Hangfire.States.IState>()),
                Times.Once);
        }

        [Fact]
        [Trait("UT", "ExportReport")]
        public async Task Handle_ExistingActiveReport_ReturnReportAlreadyExistsError()
        {
            // Arrange
            var request = new Request(ExportReportSortDirection.Asc, ExportReportSortBy.Category);
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();
            var username = "admin";

            _mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockUser.Setup(u => u.IsAuthenticated).Returns(true);
            _mockUser.Setup(u => u.UserId).Returns(userId);
            _mockUser.Setup(u => u.LocationId).Returns(locationId.ToString());
            _mockUser.Setup(u => u.Username).Returns(username);

            var existingJob = new ExportReportJob
            {
                Id = Guid.NewGuid(),
                RequestedByAdminId = userId,
                Status = ExportReportJobStatus.Processing,
                CreatedAtUtc = DateTime.UtcNow
            };

            _mockExportReportRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<ExportReportJob>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingJob);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ReportAlreadyExists, result.FirstError);

            _mockExportReportRepo.Verify(r => r.AddAsync(It.IsAny<ExportReportJob>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockBackgroundJobClient.Verify(
                x => x.Create(It.IsAny<Hangfire.Common.Job>(), It.IsAny<Hangfire.States.IState>()),
                Times.Never);
        }
    }
}
