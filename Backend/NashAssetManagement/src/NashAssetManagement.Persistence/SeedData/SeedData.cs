using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.Persistence.Builder;

namespace NashAssetManagement.Persistence.SeedData
{
    public class NamDevelopmentSeedData(AppDbContext dbContext)
    {
        public async Task SeedDataAsync(IServiceProvider serviceProvider)
        {

            // ── 1. Locations ─────────────────────────────────────────────────────
            if (!await dbContext.Locations.AnyAsync())
            {
                var locations = SeedLocationData.GetData();
                dbContext.Locations.AddRange(locations);
                await dbContext.SaveChangesAsync();
            }

            // ── 2. Categories ────────────────────────────────────────────────────
            if (!await dbContext.Categories.AnyAsync())
            {
                var categories = SeedCategoryData.GetData();
                dbContext.Categories.AddRange(categories);
                await dbContext.SaveChangesAsync();
            }

            // ── 3. Users ─────────────────────────────────────────────────────────
            if (!await dbContext.Users.AnyAsync())
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                foreach (var item in SeedUserData.GetData())
                {
                    var password = $"{item.UserName}@{item.Dob:ddMMyyyy}";

                    var user = new UserBuilder()
                        .WithId(item.Id)
                        .WithName(item.UserName)
                        .WithStaffCode(item.StaffCode)
                        .WithFirsName(item.FirstName)
                        .WithLastName(item.LastName)
                        .WithDOB(item.Dob)
                        .WithJoinDate(item.JoinedDate)
                        .WithGender(item.Gender)
                        .WithUserType(item.UserType)
                        .WithLocation(item.LocationId)
                        .WithFirstLogin(true)
                        .Build();

                    user.Email = $"{item.UserName}@nash.local";

                    var result = await userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to seed user '{item.UserName}': {errors}");
                    }
                }
            }
            else
            {
                // Patch users missing SecurityStamp (có thể xảy ra nếu seed bị interrupt)
                var usersWithNullStamp = await dbContext.Users
                    .Where(u => u.SecurityStamp == null)
                    .ToListAsync();

                if (usersWithNullStamp.Count > 0)
                {
                    foreach (var u in usersWithNullStamp)
                        u.SecurityStamp = Guid.NewGuid().ToString();

                    await dbContext.SaveChangesAsync();
                }
            }

            // ── 4. Roles ─────────────────────────────────────────────────────────
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            string[] roleNames = { ApplicationRole.Admin, ApplicationRole.Staff };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Id = roleName == ApplicationRole.Admin
                            ? Guid.Parse("20000000-0000-0000-0000-000000000001")
                            : Guid.Parse("20000000-0000-0000-0000-000000000002"),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }

            // ── 5. User Roles ────────────────────────────────────────────────────
            if (!await dbContext.UserRoles.AnyAsync())
            {
                var adminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
                var staffRoleId = Guid.Parse("20000000-0000-0000-0000-000000000002");

                var userRoles = SeedUserData.GetData()
                    .Select(u => new UserRole
                    {
                        UserId = u.Id,
                        RoleId = u.UserType == UserType.Admin ? adminRoleId : staffRoleId
                    });

                await dbContext.UserRoles.AddRangeAsync(userRoles);
                await dbContext.SaveChangesAsync();
            }

            // ── 6. Assets ────────────────────────────────────────────────────────
            if (!await dbContext.Assets.AnyAsync())
            {
                var assets = SeedAssetData.GetData();
                dbContext.Assets.AddRange(assets);
                await dbContext.SaveChangesAsync();
            }

            // ── 7. Assignments ───────────────────────────────────────────────────
            if (!await dbContext.Assignments.AnyAsync())
            {
                dbContext.ChangeTracker.Clear();
                var assignments = SeedAssignmentData.GetData();
                await dbContext.Assignments.AddRangeAsync(assignments);
                await dbContext.SaveChangesAsync();
            }

            // ── 8. Return Requests ───────────────────────────────────────────────
            if (!await dbContext.ReturnRequests.AnyAsync())
            {
                dbContext.ChangeTracker.Clear();
                var returnRequests = SeedReturnRequestData.GetData();
                await dbContext.ReturnRequests.AddRangeAsync(returnRequests);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
