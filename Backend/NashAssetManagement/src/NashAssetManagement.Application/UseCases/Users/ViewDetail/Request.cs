using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
{
    public record Request : IRequest<ErrorOr<Response>>
    {
        public Guid Id { get; init; }
    }
}