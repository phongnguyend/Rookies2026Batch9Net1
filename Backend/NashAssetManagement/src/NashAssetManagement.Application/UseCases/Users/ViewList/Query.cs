using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using MediatR;
using NashAssetManagement.Application.Utilities;

namespace NashAssetManagement.Application.UseCases.Users.ViewList
{
    public record Query : IRequest<ErrorOr<PagedList<Response>>>
    {
        public int? PageNumber { get; init; } = 1;
        public int? PageSize { get; init;} = 10;
        public string? SearchTerm { get; init; }
        public string? Type { get; init; }
        public string? SortBy { get; init; }
        public bool? SortDesc { get; init; } = false;
    }
}