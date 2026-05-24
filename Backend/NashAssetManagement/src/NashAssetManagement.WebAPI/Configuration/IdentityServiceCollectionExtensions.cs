using Microsoft.AspNetCore.Identity;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Persistence;

namespace NashAssetManagement.WebAPI.Configuration
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddAspIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                // Lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Domain.Constants.IdentityConstants.DefaultLockoutMinute);
                options.Lockout.MaxFailedAccessAttempts = Domain.Constants.IdentityConstants.MaxFailedAttempts;
                options.Lockout.AllowedForNewUsers = true;

                // User
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Password
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Sign-in
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
