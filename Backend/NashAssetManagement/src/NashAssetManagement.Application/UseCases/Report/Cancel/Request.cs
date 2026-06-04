using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Report.Cancel
{
    public sealed record Request : IRequest<ErrorOr<Response>>;
}
