using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Application.Abstractions.DataAccess;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Application.UseCases.Users.CreateUser
{
    internal class Handler(
        UserManager<User> userManager, 
        ICurrentUser currentUser,
        IValidator<Request> validator,
        ILogger<Handler> logger,
        RoleManager<Role> roleManager
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
            
            var users = userManager.Users;

            // generate staff code
            var maxNumber = users
                .Where(x => x.StaffCode.StartsWith(CompanyConstants.StaffCode))
                .Select(x => x.StaffCode.Substring(2))
                .AsEnumerable()
                .Select(x => int.TryParse(x, out var number) ? number : 0)
                .DefaultIfEmpty(0)
                .Max();

            var staffCode = $"{CompanyConstants.StaffCode}{maxNumber + 1:D4}";

            // generate username
            var firstNamePart = request.FirstName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Last();

            var lastNameParts = request.LastName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x[0]);

            // return by tuantt (first name: Phuong Tuan, last name: Trinh Tran)
            var baseUsername = $"{firstNamePart}{string.Concat(lastNameParts)}".ToLowerInvariant();
            var username = baseUsername;
            var index = 1;

            // increase index if username exist: tuantt1
            while (await userManager.Users.AnyAsync(x => x.UserName == username, cancellationToken))
            {
                username = $"{baseUsername}{index}";
                index++;
            }

            // generate passowrd
            var password = $"{username}@{request.DayOfBirth:ddMMyyyy}";

            // find role
            var roleId = await roleManager.Roles
                .Where(x => x.Name == request.UserType.ToString())
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            

            if (roleId == Guid.Empty)
            {
                return Errors.RoleNotFound;
            }

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
                Email = $"{username}@{CompanyConstants.EmailDomain}",

                // assign role
                UserRoles =
                [
                    new UserRole{ RoleId = roleId}
                ]
            };

            try
            {
                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    var errorCodes = string.Join(", ", createResult.Errors.Select(e => e.Code));

                    logger.LogError(
                        "Create user failed. Errors: {Errors}",
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