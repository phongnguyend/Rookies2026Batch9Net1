using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.AppNamingFormat;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;

namespace NashAssetManagement.Application.UseCases.Users.CreateUser
{
    internal class Handler(
        UserManager<User> userManager, 
        ICurrentUser currentUser,
        IValidator<Request> validator,
        ILogger<Handler> logger,
        IAppNamingFormat appNamingFormat
    )
    : IRequestHandler<Request, ErrorOr<Response>>
    {
        public async Task<ErrorOr<Response>> Handle(Request orgReq, CancellationToken cancellationToken)
        {
            // pre-cleanning
            var request = orgReq with
            {
                FirstName = orgReq.FirstName.Trim(),
                LastName = orgReq.LastName.Trim()
            };

            // validation
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // check current user
            if (currentUser is null || currentUser.UserId is null)
            {
                return Errors.UserNotFound;
            }

            var admin = await userManager.Users
                .Where(x => x.Id == currentUser.UserId)
                .Select(x => new
                {
                    x.Id,
                    x.LocationId
                })
                .FirstOrDefaultAsync(cancellationToken);
                    

            if (admin is null)
            {
                return Errors.UserNotFound;
            }
            
            // generate staff code
            var maxNumber = userManager.Users
                .Where(x => x.StaffCode.StartsWith(CompanyConstants.StaffCode))
                .Select(x => x.StaffCode.Substring(2))
                .AsEnumerable()
                .Select(x => int.TryParse(x, out var number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max();

            var staffCode = appNamingFormat.GetStaffCode(maxNumber + 1);

            // generate username
            var baseUsername = appNamingFormat.GetBaseUserName(request.FirstName, request.LastName);

            var existingUsernames = await userManager.Users
                .Where(x => x.UserName != null && x.UserName.StartsWith(baseUsername))
                .Select(x => x.UserName!)
                .ToListAsync(cancellationToken);

            var username = appNamingFormat.GetUniqueUserName(baseUsername, existingUsernames);

            // generate passowrd
            var password = appNamingFormat.GetPassword(username, request.DayOfBirth);

            var user = new User
            {
                Id = Guid.NewGuid(),
                StaffCode = staffCode,
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DayOfBirth,
                JoinedAtUtc = request.JoinedDate.ToUniversalTime(),
                Gender = request.Gender,
                UserType = request.UserType,
                LocationId = admin.LocationId,
                IsFirstLogin = true,
                CreatedAtUtc = DateTime.UtcNow,

                // IdentityUser required valid email
                Email = appNamingFormat.GetEmail(username),
            };

            try
            {
                // add user
                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    var errorCodes = string.Join(", ", createResult.Errors.Select(e => e.Code));

                    logger.LogError(
                        "Create user failed. Errors: {Errors}",
                        errorCodes);

                    return Errors.CreateUserFailed;
                }

                // add role
                var addRoleResult = await userManager.AddToRoleAsync(user, request.UserType.ToString());

                if (!addRoleResult.Succeeded)
                {
                    var errorCodes = string.Join(", ", addRoleResult.Errors.Select(e => e.Code));

                    logger.LogError(
                        "Assign role failed. User: {UserId}, Errors: {Errors}",
                        user.Id,
                        errorCodes);

                    return Errors.CreateUserFailed;
                }

                logger.LogInformation(
                    "User {UserId} created successfully by admin {AdminId}",
                    user.Id,
                    admin.Id);

                return new Response(
                    user.Id,
                    user.StaffCode,
                    user.UserName!,
                    user.FirstName,
                    user.LastName,
                    user.DateOfBirth,
                    user.JoinedAtUtc,
                    user.UserType.ToString(),
                    user.Gender.ToString()
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred while creating user");
                return Errors.UnexpectedError;
            }
        }
    }
}
