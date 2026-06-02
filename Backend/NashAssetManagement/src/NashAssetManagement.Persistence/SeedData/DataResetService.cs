using Microsoft.EntityFrameworkCore;

namespace NashAssetManagement.Persistence.SeedData
{
    public class DataResetService
    {
        readonly AppDbContext _dbContext;

        public DataResetService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ResetDataAsync(CancellationToken cancellationToken = default)
        {
            _dbContext.ReturnRequests.RemoveRange(
                await _dbContext.ReturnRequests.ToListAsync(cancellationToken));

            _dbContext.Assignments.RemoveRange(
                await _dbContext.Assignments.ToListAsync(cancellationToken));

            _dbContext.Assets.RemoveRange(
                await _dbContext.Assets.ToListAsync(cancellationToken));

            _dbContext.UserRoles.RemoveRange(
                await _dbContext.UserRoles.ToListAsync(cancellationToken));

            _dbContext.Roles.RemoveRange(
                await _dbContext.Roles.ToListAsync(cancellationToken));

            _dbContext.RefreshTokens.RemoveRange(
                await _dbContext.RefreshTokens.ToListAsync(cancellationToken));

            _dbContext.Users.RemoveRange(
                await _dbContext.Users.ToListAsync(cancellationToken));

            _dbContext.Categories.RemoveRange(
                await _dbContext.Categories.ToListAsync(cancellationToken));

            _dbContext.Locations.RemoveRange(
                await _dbContext.Locations.ToListAsync(cancellationToken));

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
