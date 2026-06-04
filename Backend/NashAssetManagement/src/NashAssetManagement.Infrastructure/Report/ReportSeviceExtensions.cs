using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Application.Abstractions.File;
using NashAssetManagement.Application.Abstractions.Report;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.Infrastructure.Report
{
    public static class ReportServiceExtensions
    {
        public static IServiceCollection AddReportServices(
            this IServiceCollection services)
        {
            // Later if more type of report using same excel interface, register as KeyedScoped
            services.AddKeyedScoped<IExcelGenerator, ReportExcelGenerator>(AppCts.Services.ReportExcel);
            services.AddScoped<IReportExportJobService, ReportExportJobService>();
            services.AddScoped<IReportFileNameService, ReportFileNameService>();

            return services;
        }
    }
}
