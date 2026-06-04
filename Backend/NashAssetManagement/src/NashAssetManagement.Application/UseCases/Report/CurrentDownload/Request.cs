using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Report.CurrentDownload
{
    public sealed record Request : IRequest<ErrorOr<Response>>;
}