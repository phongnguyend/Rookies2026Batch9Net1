using Hangfire.Annotations;
using Hangfire.Dashboard;
using NashAssetManagement.Application.Abstractions.AppIdentity;
using NashAssetManagement.Domain.Constants;

namespace NashAssetManagement.WebAPI.Filters
{
    public sealed class HangFireAdminDashboardFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var currentUser = httpContext.RequestServices.GetRequiredService<ICurrentUser>();

            // if not authenticated, not allowed
            if (currentUser == null)
            {
                return false;
            }

            // allow admin only
            return currentUser?.IsAuthenticated == true
                && currentUser.Roles.Contains(ApplicationRole.Admin);
        }
    }
}