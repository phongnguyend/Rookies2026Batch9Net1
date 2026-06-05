using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using MediatR;
using ErrorOr;
using NashAssetManagement.Application.Utilities;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace NashAssetManagement.Application.UseCases.Users.ViewUsers
{
    internal class Handler(
        UserManager<User> userManager, 
        ICurrentUser currentUser,
        IValidator<Request> validator
    )
    : IRequestHandler<Request, ErrorOr<PagedList<Response>>>
    {
        public async Task<ErrorOr<PagedList<Response>>> Handle(
            Request request,
            CancellationToken cancellationToken)
        {
            // Check current user id
            if (currentUser.UserId == null)
                return Errors.Unauthorized();

            // Check current admin's location
            if (currentUser.LocationId == null)            
                return Errors.UserHasNoLocation();

            var validationResults = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResults.IsValid)
                throw new ValidationException(validationResults.Errors);
        
            // Get users have same location with current admin user
            var usersQuery = userManager.Users
                .Where(u => u.LocationId.ToString().Equals(currentUser.LocationId) && !u.IsDeleted)
                ;

            // Cleaned request
            var cleanedRequest = request with
            {
                SortBy = request.SortBy?.Trim(),
                PageNumber = request.PageNumber ?? 1,
                PageSize = request.PageSize ?? 10,
            };

            // Search
            if (!string.IsNullOrWhiteSpace(cleanedRequest.SearchTerm))
            {
                var searchTerm = cleanedRequest.SearchTerm.Trim().ToLowerInvariant();
                var pattern = $"%{searchTerm}%";

                usersQuery = usersQuery.Where(u =>
                    (u.StaffCode != null && EF.Functions.Like(u.StaffCode.ToLower(), pattern)) ||
                    (u.FirstName != null && EF.Functions.Like(u.FirstName.ToLower(), pattern)) ||
                    (u.LastName != null && EF.Functions.Like(u.LastName.ToLower(), pattern)) ||
                    EF.Functions.Like(((u.FirstName ?? "") + " " + (u.LastName ?? "")).ToLower(), pattern));
            }

            // Filter by user type
            if (!string.IsNullOrEmpty(cleanedRequest.Type) &&
                Enum.TryParse<UserType>(cleanedRequest.Type, ignoreCase: true, out var userType))
            {
                usersQuery = usersQuery.Where(u => u.UserType == userType);
            }

            // Apply sorting
            var sortBy = cleanedRequest.SortBy?.Trim().ToLowerInvariant();
            var sortDesc = cleanedRequest.SortDesc.GetValueOrDefault();

            if (string.IsNullOrEmpty(sortBy))
            {
                usersQuery = usersQuery.OrderByDescending(u => u.CreatedAtUtc);
            }
            else
            {
                usersQuery = sortBy switch
                {
                    "staffcode" => sortDesc
                        ? usersQuery.OrderByDescending(u => u.StaffCode)
                        : usersQuery.OrderBy(u => u.StaffCode),
                    "fullname" => sortDesc
                        ? usersQuery.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName)
                        : usersQuery.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                    "username" => sortDesc
                        ? usersQuery.OrderByDescending(u => u.UserName)
                        : usersQuery.OrderBy(u => u.UserName),
                    "joineddate" => sortDesc
                        ? usersQuery.OrderByDescending(u => u.JoinedAtUtc)
                        : usersQuery.OrderBy(u => u.JoinedAtUtc),
                    "usertype" => sortDesc
                        ? usersQuery.OrderByDescending(u => u.UserType)
                        : usersQuery.OrderBy(u => u.UserType),
                    "createddate" => sortDesc
                        ? usersQuery.OrderByDescending(u => u.CreatedAtUtc)
                        : usersQuery.OrderBy(u => u.CreatedAtUtc),
                    "updateddate" => sortDesc
                        ? usersQuery.OrderByDescending(u => u.UpdatedAtUtc)
                        : usersQuery.OrderBy(u => u.UpdatedAtUtc),
                    _ => usersQuery.OrderByDescending(u => u.CreatedAtUtc)
                };
            }

            // Apply paging
            var totalItems = await usersQuery.CountAsync(cancellationToken);
            var users = await usersQuery
            .Skip((cleanedRequest.PageNumber!.Value -1) * cleanedRequest.PageSize!.Value)
            .Take(cleanedRequest.PageSize.Value)
            .Select(u => new Response(
                u.Id,
                u.StaffCode,
                u.FirstName + " " + u.LastName,
                u.UserName ?? "",
                u.JoinedAtUtc,
                u.UserType.ToString()
            )
            {
                CanBeDisabled = !u.ReceivedAssignments.Any(a =>
                    a.State == AssignmentState.WaitingForAcceptance ||
                    a.State == AssignmentState.Accepted)
            })
            .ToListAsync(cancellationToken);
            return new PagedList<Response>(users, totalItems, cleanedRequest.PageNumber!.Value, cleanedRequest.PageSize!.Value);
        }
    }
}
