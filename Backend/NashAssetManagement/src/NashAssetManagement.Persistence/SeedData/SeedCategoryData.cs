using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Persistence.Builder;

namespace NashAssetManagement.Persistence.SeedData
{
    public static class SeedCategoryData
    {
        public static List<Category> GetData() => new List<Category>
        {
            new CategoryBuilder()
                .WithId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                .WithName("Laptop")
                .WithPrefix("LA")
                .Build(),

            new CategoryBuilder()
                .WithId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                .WithName("Monitor")
                .WithPrefix("MO")
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
                .WithPrefix("BA")
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
    }
}
