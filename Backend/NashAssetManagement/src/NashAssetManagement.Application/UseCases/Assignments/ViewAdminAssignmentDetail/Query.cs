using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Assignments.GetById
{
    public record Query(Guid Id): IRequest<ErrorOr<Response>>;
}
