using ErrorOr;
using MediatR;

namespace NashAssetManagement.Application.UseCases.Users.ViewDetail
{
    public record Query : IRequest<ErrorOr<Response>>
    {
        public Guid Id { get; init; }
    }
}