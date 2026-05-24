using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using MediatR;
using ErrorOr;
using NashAssetManagement.Application.Utilities;
using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using Microsoft.EntityFrameworkCore;

namespace NashAssetManagement.Application.UseCases.Users.ViewList
{
    internal class Handler(UserManager<User> userManager, ICurrentUser currentUser)
    : IRequestHandler<Query, ErrorOr<PagedList<Response>>>
    {
        public async Task<ErrorOr<PagedList<Response>>> Handle(
            Query query,
            CancellationToken cancellationToken)
        {
            // Get current admin's location
            var currentAdminUserId = currentUser.UserId;
            if (currentAdminUserId == null)
                return Errors.Unauthorized();
            var currentAdmin = await userManager.FindByIdAsync(currentAdminUserId.ToString());
            if (currentAdmin == null)
                return Errors.UserNotFound();

            // Get users have same location with current admin user
            var usersQuery = userManager.Users
                .Where(u => u.LocationId.Equals(currentAdmin.LocationId))
                ;

            // Search
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.Trim().ToLowerInvariant();
                var pattern = $"%{searchTerm}%";

                usersQuery = usersQuery.Where(u =>
                    (u.StaffCode != null && EF.Functions.Like(u.StaffCode.ToLower(), pattern)) ||
                    (u.FirstName != null && EF.Functions.Like(u.FirstName.ToLower(), pattern)) ||
                    (u.LastName != null && EF.Functions.Like(u.LastName.ToLower(), pattern)));
            }

            // Filter by user type
            if (!string.IsNullOrEmpty(query.Type) &&
                Enum.TryParse<UserType>(query.Type, ignoreCase: true, out var userType))
            {
                usersQuery = usersQuery.Where(u => u.UserType == userType);
            }

            // Apply sorting
            var sortBy = query.SortBy?.Trim().ToLowerInvariant();
            var sortDesc = query.SortDesc.GetValueOrDefault();

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
            .Skip((query.PageNumber!.Value -1) * query.PageSize!.Value)
            .Take(query.PageSize.Value)
            .Select(u => new Response(
                u.Id,
                u.StaffCode,
                u.FirstName + " " + u.LastName,
                u.UserName ?? "",
                u.JoinedAtUtc.ToString("yyyy-MM-dd"),
                u.UserType.ToString()
            )
            {
                CanBeDisabled = !u.ReceivedAssignments.Any(a =>
                    a.State == AssignmentState.WaitingForAcceptance ||
                    a.State == AssignmentState.Accepted)
            })
            .ToListAsync(cancellationToken);
            return new PagedList<Response>(users, totalItems, query.PageNumber!.Value, query.PageSize!.Value);
        }
    }
}