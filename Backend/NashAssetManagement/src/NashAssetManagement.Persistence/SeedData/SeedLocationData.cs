using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Persistence.Builder;

namespace NashAssetManagement.Persistence.SeedData
{
    public static class SeedLocationData
    {
        public static List<Location> GetData() => new List<Location>
        {
            new LocationBuilder()
                .WithId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                .WithName("Ha Noi")
                .WithPrefix("HN")
                .Build(),

            new LocationBuilder()
                .WithId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                .WithName("Ho Chi Minh")
                .WithPrefix("HCM")
                .Build()
        };
    }
}
