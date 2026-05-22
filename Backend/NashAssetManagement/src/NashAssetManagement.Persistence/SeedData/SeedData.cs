using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Entities.Identity;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.Persistence.Builder;

namespace NashAssetManagement.Persistence.SeedData
{
    public class NAMDbContextSeedData(AppDbContext dbContext)
    {
        public async Task SeedDataAsync()
        {

            #region Location
            var LocationsData = new List<Location>
            {
                new LocationBuilder().WithId(Guid.Parse("11111111-1111-1111-1111-111111111111")).WithName("Ha Noi").WithPrefix("HN").Build(),
                new LocationBuilder().WithId(Guid.Parse("22222222-2222-2222-2222-222222222222")).WithName("Ho Chi Minh").WithPrefix("HCM").Build()
            };
            #endregion
            #region Category
            var CategoriesData = new List<Category>
            {
                new CategoryBuilder()
                .WithId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                .WithName("Laptop")
                .WithPrefix("LT")
                .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithName("Monitor")
                    .WithPrefix("MN")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithName("Keyboard")
                    .WithPrefix("KB")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithName("Mouse")
                    .WithPrefix("MS")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithName("Bluetooth Mouse")
                    .WithPrefix("BM")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithName("Battery Monitor")
                    .WithPrefix("BM1")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithName("Printer")
                    .WithPrefix("PR")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithName("Scanner")
                    .WithPrefix("SC")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithName("Projector")
                    .WithPrefix("PJ")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithName("Tablet")
                    .WithPrefix("TB")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithName("Desktop Computer")
                    .WithPrefix("DC")
                    .Build(),

                new CategoryBuilder()
                    .WithId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithName("Network Switch")
                    .WithPrefix("NS")
                    .Build(),
                };
            #endregion

            #region User

            #endregion
            if (!await dbContext.Locations.AnyAsync())
            {
                dbContext.Locations.AddRange(LocationsData);
                await dbContext.SaveChangesAsync();
            }

            if (!await dbContext.Categories.AnyAsync())
            {
                dbContext.Categories.AddRange(CategoriesData);
                await dbContext.SaveChangesAsync();
            }
            return;
        }
    }
}