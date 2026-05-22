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

            #region  User
            var users = new List<User>();

            var haNoiId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var hcmId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var passwordHasher = new PasswordHasher<User>();

            var seedUsers = new List<(Guid Id, string UserName, string StaffCode, string FirstName, string LastName, Gender Gender, DateTime Dob, DateTime JoinedDate, Guid LocationId, UserType UserType)>
            {
                // HA NOI ADMINS
                (Guid.Parse("10000000-0000-0000-0000-000000000001"), "binhnv", "SD0001", "Binh", "Nguyen Van", Gender.Male, new DateTime(1993, 01, 20), new DateTime(2018, 01, 20), haNoiId, UserType.Admin),
                (Guid.Parse("10000000-0000-0000-0000-000000000002"), "huongtt", "SD0002", "Huong", "Tran Thi", Gender.Female, new DateTime(1994, 03, 15), new DateTime(2019, 04, 10), haNoiId, UserType.Admin),
                (Guid.Parse("10000000-0000-0000-0000-000000000003"), "datlq", "SD0003", "Dat", "Le Quang", Gender.Male, new DateTime(1992, 07, 11), new DateTime(2017, 08, 01), haNoiId, UserType.Admin),

                // HA NOI STAFF
                (Guid.Parse("10000000-0000-0000-0000-000000000004"), "linhpt", "SD0004", "Linh", "Pham Thi", Gender.Female, new DateTime(1998, 02, 18), new DateTime(2021, 03, 10), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000005"), "khanhdm", "SD0005", "Khanh", "Do Minh", Gender.Male, new DateTime(1997, 09, 22), new DateTime(2020, 10, 15), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000006"), "maivt", "SD0006", "Mai", "Vo Thi", Gender.Female, new DateTime(1996, 12, 09), new DateTime(2019, 12, 20), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000007"), "tuanba", "SD0007", "Tuan", "Bui Anh", Gender.Male, new DateTime(1995, 06, 30), new DateTime(2018, 07, 15), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000008"), "ngandt", "SD0008", "Ngan", "Dang Thu", Gender.Female, new DateTime(1999, 08, 14), new DateTime(2022, 01, 05), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000009"), "phonghg", "SD0009", "Phong", "Hoang Gia", Gender.Male, new DateTime(1991, 11, 25), new DateTime(2016, 05, 22), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000010"), "vybn", "SD0010", "Vy", "Bao Ngo", Gender.Female, new DateTime(1998, 05, 17), new DateTime(2021, 09, 12), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000011"), "hungpd", "SD0011", "Hung", "Phan Duc", Gender.Male, new DateTime(1990, 04, 02), new DateTime(2015, 04, 20), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000012"), "thaoln", "SD0012", "Thao", "Ly Ngoc", Gender.Female, new DateTime(1997, 01, 28), new DateTime(2020, 02, 11), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000013"), "quantm", "SD0013", "Quan", "Truong Minh", Gender.Male, new DateTime(1993, 10, 13), new DateTime(2018, 11, 01), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000014"), "yendt", "SD0014", "Yen", "Duong Thanh", Gender.Female, new DateTime(1996, 07, 07), new DateTime(2019, 08, 18), haNoiId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000015"), "namct", "SD0015", "Nam", "Cao Tuan", Gender.Male, new DateTime(1994, 09, 19), new DateTime(2019, 10, 05), haNoiId, UserType.Staff),

                // HCM ADMINS
                (Guid.Parse("10000000-0000-0000-0000-000000000016"), "annh", "SD0016", "An", "Nguyen Hoang", Gender.Male, new DateTime(1992, 04, 12), new DateTime(2017, 06, 15), hcmId, UserType.Admin),
                (Guid.Parse("10000000-0000-0000-0000-000000000017"), "trangpn", "SD0017", "Trang", "Pham Ngoc", Gender.Female, new DateTime(1995, 02, 24), new DateTime(2019, 03, 08), hcmId, UserType.Admin),
                (Guid.Parse("10000000-0000-0000-0000-000000000018"), "kiettm", "SD0018", "Kiet", "Tran Minh", Gender.Male, new DateTime(1991, 08, 06), new DateTime(2016, 09, 01), hcmId, UserType.Admin),

                // HCM STAFF
                (Guid.Parse("10000000-0000-0000-0000-000000000019"), "lanlt", "SD0019", "Lan", "Le Thi", Gender.Female, new DateTime(1997, 03, 09), new DateTime(2020, 04, 12), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000020"), "hieuvt", "SD0020", "Hieu", "Vo Thanh", Gender.Male, new DateTime(1996, 12, 21), new DateTime(2020, 01, 17), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000021"), "mydb", "SD0021", "My", "Dang Bao", Gender.Female, new DateTime(1998, 06, 11), new DateTime(2021, 07, 22), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000022"), "ducbg", "SD0022", "Duc", "Bui Gia", Gender.Male, new DateTime(1993, 05, 28), new DateTime(2018, 06, 30), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000023"), "nhiht", "SD0023", "Nhi", "Ho Thi", Gender.Female, new DateTime(1999, 09, 14), new DateTime(2022, 02, 15), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000024"), "taipa", "SD0024", "Tai", "Phan Anh", Gender.Male, new DateTime(1990, 01, 03), new DateTime(2015, 03, 10), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000025"), "chaunm", "SD0025", "Chau", "Ngo Minh", Gender.Female, new DateTime(1997, 07, 18), new DateTime(2020, 09, 09), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000026"), "vietlq", "SD0026", "Viet", "Ly Quoc", Gender.Male, new DateTime(1994, 11, 29), new DateTime(2019, 01, 15), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000027"), "hanhdk", "SD0027", "Hanh", "Duong Kim", Gender.Female, new DateTime(1995, 10, 10), new DateTime(2019, 11, 11), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000028"), "sontd", "SD0028", "Son", "Trinh Duc", Gender.Male, new DateTime(1992, 02, 16), new DateTime(2017, 03, 18), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000029"), "ngamt", "SD0029", "Nga", "Mai Thu", Gender.Female, new DateTime(1998, 08, 27), new DateTime(2021, 10, 20), hcmId, UserType.Staff),
                (Guid.Parse("10000000-0000-0000-0000-000000000030"), "longcv", "SD0030", "Long", "Cao Van", Gender.Male, new DateTime(1993, 12, 05), new DateTime(2018, 12, 01), hcmId, UserType.Staff),
            };

            foreach (var item in seedUsers)
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

                user.Email = $"{item.UserName}@nam.local";
                user.NormalizedEmail = user.Email.ToUpper();
                user.NormalizedUserName = item.UserName.ToUpper();

                user.PasswordHash = passwordHasher.HashPassword(user, password);

                users.Add(user);
            }
            #endregion

            if (!await dbContext.Locations.AnyAsync())
            {
                dbContext.Locations.AddRange(LocationsData);
                await dbContext.SaveChangesAsync();
            }
            if (!await dbContext.Users.AnyAsync())
            {
                dbContext.Users.AddRange(users);
                await dbContext.SaveChangesAsync();
            }
            if (!await dbContext.Categories.AnyAsync())
            {
                dbContext.Categories.AddRange(CategoriesData);
                await dbContext.SaveChangesAsync();
            }

            #region RoleIdentity
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            string[] roleNames =
            {
                ApplicationRole.Admin,
                ApplicationRole.Staff
            };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Id = roleName == "Admin"
                            ? Guid.Parse("20000000-0000-0000-0000-000000000001")
                            : Guid.Parse("20000000-0000-0000-0000-000000000002"),

                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }
            #endregion

            #region UserRoleIdentity
            if (!await dbContext.UserRoles.AnyAsync())
            {
                var adminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
                var staffRoleId = Guid.Parse("20000000-0000-0000-0000-000000000002");

                var userRoleMappings = new (Guid UserId, UserType Type)[]
                {
                    // ===== HA NOI ADMINS =====
                    (Guid.Parse("10000000-0000-0000-0000-000000000001"), UserType.Admin),
                    (Guid.Parse("10000000-0000-0000-0000-000000000002"), UserType.Admin),
                    (Guid.Parse("10000000-0000-0000-0000-000000000003"), UserType.Admin),

                    // ===== HA NOI STAFF =====
                    (Guid.Parse("10000000-0000-0000-0000-000000000004"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000005"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000006"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000007"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000008"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000009"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000010"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000011"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000012"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000013"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000014"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000015"), UserType.Staff),

                    // ===== HCM ADMINS =====
                    (Guid.Parse("10000000-0000-0000-0000-000000000016"), UserType.Admin),
                    (Guid.Parse("10000000-0000-0000-0000-000000000017"), UserType.Admin),
                    (Guid.Parse("10000000-0000-0000-0000-000000000018"), UserType.Admin),

                    // ===== HCM STAFF =====
                    (Guid.Parse("10000000-0000-0000-0000-000000000019"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000020"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000021"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000022"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000023"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000024"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000025"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000026"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000027"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000028"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000029"), UserType.Staff),
                    (Guid.Parse("10000000-0000-0000-0000-000000000030"), UserType.Staff),
                };

                var userRoles = userRoleMappings.Select(u => new UserRole
                {
                    UserId = u.UserId,
                    RoleId = u.Type == UserType.Admin ? adminRoleId : staffRoleId
                });

                await dbContext.UserRoles.AddRangeAsync(userRoles);
                await dbContext.SaveChangesAsync();
            }
            #endregion

            #region Asset
            var assets = new List<Asset>
            {
               #region Asset Ha Noi
                // LAPTOP (LT)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000001"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Laptop HP ProBook 450 G9")
                    .WithAssetCode("LT000001")
                    .WithAssetSpecification("Intel Core i5-1235U, 8GB RAM, 256GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000002"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Laptop Dell Latitude 5540")
                    .WithAssetCode("LT000002")
                    .WithAssetSpecification("Intel Core i7-1365U, 16GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000003"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Laptop Lenovo ThinkPad E15 Gen 4")
                    .WithAssetCode("LT000003")
                    .WithAssetSpecification("AMD Ryzen 5 5625U, 16GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000004"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Laptop Asus VivoBook 15 X1502")
                    .WithAssetCode("LT000004")
                    .WithAssetSpecification("Intel Core i5-12500H, 8GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000005"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Laptop Acer Aspire 5 A515-57")
                    .WithAssetCode("LT000005")
                    .WithAssetSpecification("Intel Core i5-12450H, 8GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // MONITOR (MN)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000006"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Monitor Dell UltraSharp U2422H")
                    .WithAssetCode("MN000001")
                    .WithAssetSpecification("24\" FHD IPS, 1920x1080, 60Hz, HDMI, DisplayPort, USB-C")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000007"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Monitor LG 27UK850-W")
                    .WithAssetCode("MN000002")
                    .WithAssetSpecification("27\" 4K UHD IPS, 3840x2160, 60Hz, HDMI, DisplayPort, USB-C")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000008"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Monitor Samsung F24T450FQE")
                    .WithAssetCode("MN000003")
                    .WithAssetSpecification("24\" FHD IPS, 1920x1080, 75Hz, HDMI, DisplayPort, Adjustable Stand")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000009"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Monitor BenQ GW2480")
                    .WithAssetCode("MN000004")
                    .WithAssetSpecification("24\" FHD IPS, 1920x1080, 60Hz, HDMI, VGA, Eye-Care Technology")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000010"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Monitor AOC 22B2HM")
                    .WithAssetCode("MN000005")
                    .WithAssetSpecification("21.5\" FHD VA, 1920x1080, 75Hz, HDMI, VGA, Flicker-Free")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 7, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 7, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // KEYBOARD (KB)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000011"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Keyboard Logitech MK270 Wireless")
                    .WithAssetCode("KB000001")
                    .WithAssetSpecification("Full-size, Wireless 2.4GHz, USB Receiver, Battery Life 24 months")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000012"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Keyboard Dell KB216 Wired")
                    .WithAssetCode("KB000002")
                    .WithAssetSpecification("Full-size, USB, Membrane Keys, Spill-resistant, 104 Keys")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000013"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Keyboard HP 125 Wired")
                    .WithAssetCode("KB000003")
                    .WithAssetSpecification("Full-size, USB, Quiet Keys, Adjustable Tilt Legs, 104 Keys")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000014"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Keyboard Rapoo E9050G Wireless")
                    .WithAssetCode("KB000004")
                    .WithAssetSpecification("Slim, Wireless 2.4GHz, Multi-mode, USB Receiver, Scissor Switch")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000015"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Keyboard Genius KB-110X Wired")
                    .WithAssetCode("KB000005")
                    .WithAssetSpecification("Full-size, USB, Membrane, Spill-resistant, 104 Keys")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // MOUSE (MS)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000016"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Mouse Logitech M100 Wired")
                    .WithAssetCode("MS000001")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Ambidextrous, Plug-and-Play")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000017"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Mouse Dell MS116 Wired")
                    .WithAssetCode("MS000002")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Right-handed, Scroll Wheel")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000018"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Mouse HP X500 Wired")
                    .WithAssetCode("MS000003")
                    .WithAssetSpecification("USB, Optical, 800/1200/1600 DPI, Ergonomic, Scroll Wheel")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000019"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Mouse Rapoo N1130 Wired")
                    .WithAssetCode("MS000004")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Ambidextrous, 3-Button Design")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000020"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Mouse Genius DX-110 Wired")
                    .WithAssetCode("MS000005")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Scroll Wheel, Plug-and-Play")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // BLUETOOTH MOUSE (BM) 
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000021"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Bluetooth Mouse Logitech M650 L")
                    .WithAssetCode("BM000001")
                    .WithAssetSpecification("Bluetooth 5.0, 400-4000 DPI, Silent Click, 24-Month Battery Life")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000022"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Bluetooth Mouse Microsoft Arc")
                    .WithAssetCode("BM000002")
                    .WithAssetSpecification("Bluetooth 5.0, BlueTrack, Foldable Design, 6-Month Battery Life")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000023"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Bluetooth Mouse HP 240 Pike Silver")
                    .WithAssetCode("BM000003")
                    .WithAssetSpecification("Bluetooth 5.1, 1600 DPI, Silent Click, 16-Month Battery Life")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000024"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Bluetooth Mouse Rapoo M650 Silent")
                    .WithAssetCode("BM000004")
                    .WithAssetSpecification("Bluetooth 3.0/5.0, 1600 DPI, Silent Buttons, 9-Month Battery Life")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000025"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Bluetooth Mouse Dell MS700")
                    .WithAssetCode("BM000005")
                    .WithAssetSpecification("Bluetooth 5.0, 1600 DPI, Ergonomic, 36-Month Battery Life")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // BATTERY MONITOR (BM1) 
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000026"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Battery Monitor Eaton 5E 1100i USB")
                    .WithAssetCode("BM1000001")
                    .WithAssetSpecification("1100VA/660W, USB, 4 Output Sockets, Protection Time 9 min at full load")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 28, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 28, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000027"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Battery Monitor APC Back-UPS BX1000M")
                    .WithAssetCode("BM1000002")
                    .WithAssetSpecification("1000VA/600W, USB, 8 Output Sockets, LCD Display, AVR")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000028"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Battery Monitor CyberPower CP900EPFCLCD")
                    .WithAssetCode("BM1000003")
                    .WithAssetSpecification("900VA/540W, USB, 8 Output Sockets, LCD Panel, Pure Sine Wave")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000029"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Battery Monitor Santak TG-BOX 850")
                    .WithAssetCode("BM1000004")
                    .WithAssetSpecification("850VA/510W, USB, 4 Output Sockets, AVR, Backup Time 9 min")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000030"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Battery Monitor Ares AR-800VA")
                    .WithAssetCode("BM1000005")
                    .WithAssetSpecification("800VA/480W, USB, 4 Output Sockets, AVR, Backup Time 7 min")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // PRINTER (PR) 
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000031"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Printer HP LaserJet Pro M404dn")
                    .WithAssetCode("PR000001")
                    .WithAssetSpecification("A4, Mono Laser, 38ppm, Duplex, USB, LAN, 250-Sheet Tray")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000032"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Printer Canon imageCLASS LBP6030")
                    .WithAssetCode("PR000002")
                    .WithAssetSpecification("A4, Mono Laser, 18ppm, USB, 150-Sheet Tray, Compact Design")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 14, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 14, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000033"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Printer Epson EcoTank L3210")
                    .WithAssetCode("PR000003")
                    .WithAssetSpecification("A4, Color Inkjet, 10ppm Black/5ppm Color, USB, Ink Tank System")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000034"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Printer Brother DCP-L2540DW")
                    .WithAssetCode("PR000004")
                    .WithAssetSpecification("A4, Mono Laser MFP, 30ppm, Duplex, WiFi, USB, LAN, 250-Sheet Tray")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000035"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Printer Samsung SL-M2020W")
                    .WithAssetCode("PR000005")
                    .WithAssetSpecification("A4, Mono Laser, 21ppm, WiFi, USB, 150-Sheet Tray")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // SCANNER (SC)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000036"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Scanner Canon DR-C225W II")
                    .WithAssetCode("SC000001")
                    .WithAssetSpecification("A4, 25ppm/50ipm, ADF 30-Sheet, WiFi, USB, Duplex, CIS Sensor")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000037"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Scanner Epson DS-530 II")
                    .WithAssetCode("SC000002")
                    .WithAssetSpecification("A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000038"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Scanner Fujitsu fi-7030")
                    .WithAssetCode("SC000003")
                    .WithAssetSpecification("A4, 27ppm/54ipm, ADF 50-Sheet, USB, Duplex, CCD Sensor, 600 DPI")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000039"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Scanner HP ScanJet Pro 2000 s2")
                    .WithAssetCode("SC000004")
                    .WithAssetSpecification("A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000040"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Scanner Brother ADS-1200")
                    .WithAssetCode("SC000005")
                    .WithAssetSpecification("A4, 25ppm/50ipm, ADF 20-Sheet, USB, Duplex, CIS Sensor, 600 DPI")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // PROJECTOR (PJ) 
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000041"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Projector Epson EB-X49")
                    .WithAssetCode("PJ000001")
                    .WithAssetSpecification("XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, Lamp 12000h")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000042"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Projector Benq MX550")
                    .WithAssetCode("PJ000002")
                    .WithAssetSpecification("XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, SmartEco Mode")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000043"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Projector ViewSonic PA503X")
                    .WithAssetCode("PJ000003")
                    .WithAssetSpecification("XGA 1024x768, 3800 Lumens, HDMI, USB, VGA, Lamp 15000h")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000044"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Projector Optoma X400LVe")
                    .WithAssetCode("PJ000004")
                    .WithAssetSpecification("XGA 1024x768, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h ECO")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000045"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Projector Acer X1526HK")
                    .WithAssetCode("PJ000005")
                    .WithAssetSpecification("FHD 1920x1080, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 19, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 19, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // TABLET (TB) 
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000046"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Tablet Samsung Galaxy Tab A8")
                    .WithAssetCode("TB000001")
                    .WithAssetSpecification("10.5\" TFT, Unisoc T618, 4GB RAM, 64GB, WiFi, Android 11")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000047"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Tablet Lenovo Tab M10 Plus Gen 3")
                    .WithAssetCode("TB000002")
                    .WithAssetSpecification("10.61\" 2K IPS, Snapdragon 680, 4GB RAM, 128GB, WiFi, Android 12")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000048"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Tablet Huawei MatePad 11.5")
                    .WithAssetCode("TB000003")
                    .WithAssetSpecification("11.5\" IPS 120Hz, Snapdragon 7 Gen 1, 8GB RAM, 128GB, WiFi")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000049"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Tablet Xiaomi Pad 6")
                    .WithAssetCode("TB000004")
                    .WithAssetSpecification("11\" IPS 144Hz, Snapdragon 870, 8GB RAM, 256GB, WiFi, MIUI 14")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000050"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Tablet Amazon Fire HD 10")
                    .WithAssetCode("TB000005")
                    .WithAssetSpecification("10.1\" FHD IPS, Octa-Core 2.0GHz, 3GB RAM, 32GB, WiFi, FireOS")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // DESKTOP COMPUTER (DC) 
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000051"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Desktop Dell OptiPlex 3000 MT")
                    .WithAssetCode("DC000001")
                    .WithAssetSpecification("Intel Core i5-12500, 8GB RAM, 256GB SSD, Intel UHD 770, Win 11 Pro")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000052"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Desktop HP EliteDesk 800 G9 SFF")
                    .WithAssetCode("DC000002")
                    .WithAssetSpecification("Intel Core i7-12700, 16GB RAM, 512GB SSD, Intel UHD 770, Win 11 Pro")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000053"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Desktop Lenovo ThinkCentre M70s Gen 3")
                    .WithAssetCode("DC000003")
                    .WithAssetSpecification("Intel Core i5-12400, 8GB RAM, 256GB SSD, Intel UHD 730, Win 11 Pro")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000054"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Desktop Asus ExpertCenter D5 SFF D500SC")
                    .WithAssetCode("DC000004")
                    .WithAssetSpecification("Intel Core i3-10105, 8GB RAM, 256GB SSD, Intel UHD 630, Win 11 Pro")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000055"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Desktop Acer Veriton M6680G")
                    .WithAssetCode("DC000005")
                    .WithAssetSpecification("Intel Core i5-10400, 8GB RAM, 256GB SSD, Intel UHD 630, Win 10 Pro")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // NETWORK SWITCH (NS) 
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000056"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Network Switch Cisco CBS110-16T")
                    .WithAssetCode("NS000001")
                    .WithAssetSpecification("16-Port Gigabit, Unmanaged, Desktop, Fanless, QoS, IEEE 802.3az")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000057"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Network Switch TP-Link TL-SG1024D")
                    .WithAssetCode("NS000002")
                    .WithAssetSpecification("24-Port Gigabit, Unmanaged, Desktop/Rack-mount, Fanless, IEEE 802.3az")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000058"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Network Switch D-Link DGS-1016D")
                    .WithAssetCode("NS000003")
                    .WithAssetSpecification("16-Port Gigabit, Unmanaged, Desktop, Fanless, Auto MDI/MDIX")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000059"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Network Switch Netgear GS308E")
                    .WithAssetCode("NS000004")
                    .WithAssetSpecification("8-Port Gigabit, Smart Managed, Desktop, QoS, VLAN, IGMP Snooping")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000060"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("11111111-1111-1111-1111-111111111111"))
                    .WithAssetName("Network Switch MikroTik CRS112-8G-4S-IN")
                    .WithAssetCode("NS000005")
                    .WithAssetSpecification("8-Port Gigabit + 4 SFP, Managed, Desktop, RouterOS/SwitchOS, PoE")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 29, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 29, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
                #endregion
 
               #region Assets - Ho Chi Minh
                // LAPTOP (LA)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000061"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Laptop HP ProBook 450 G9")
                    .WithAssetCode("LT000006")
                    .WithAssetSpecification("Intel Core i5-1235U, 8GB RAM, 256GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000062"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Laptop Dell Latitude 5540")
                    .WithAssetCode("LT000007")
                    .WithAssetSpecification("Intel Core i7-1365U, 16GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000063"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Laptop Lenovo ThinkPad E15 Gen 4")
                    .WithAssetCode("LT000008")
                    .WithAssetSpecification("AMD Ryzen 5 5625U, 16GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000064"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Laptop Asus VivoBook 15 X1502")
                    .WithAssetCode("LT000009")
                    .WithAssetSpecification("Intel Core i5-12500H, 8GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000065"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Laptop Acer Aspire 5 A515-57")
                    .WithAssetCode("LT000010")
                    .WithAssetSpecification("Intel Core i5-12450H, 8GB RAM, 512GB SSD, 15.6\" FHD")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // MONITOR (MO)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000066"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Monitor Dell UltraSharp U2422H")
                    .WithAssetCode("MN000006")
                    .WithAssetSpecification("24\" FHD IPS, 1920x1080, 60Hz, HDMI, DisplayPort, USB-C")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000067"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Monitor LG 27UK850-W")
                    .WithAssetCode("MN000007")
                    .WithAssetSpecification("27\" 4K UHD IPS, 3840x2160, 60Hz, HDMI, DisplayPort, USB-C")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000068"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Monitor Samsung F24T450FQE")
                    .WithAssetCode("MN000008")
                    .WithAssetSpecification("24\" FHD IPS, 1920x1080, 75Hz, HDMI, DisplayPort, Adjustable Stand")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000069"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Monitor BenQ GW2480")
                    .WithAssetCode("MN000009")
                    .WithAssetSpecification("24\" FHD IPS, 1920x1080, 60Hz, HDMI, VGA, Eye-Care Technology")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000070"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Monitor AOC 22B2HM")
                    .WithAssetCode("MN000010")
                    .WithAssetSpecification("21.5\" FHD VA, 1920x1080, 75Hz, HDMI, VGA, Flicker-Free")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 7, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 7, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // KEYBOARD (KE)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000071"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Keyboard Logitech MK270 Wireless")
                    .WithAssetCode("KB000006")
                    .WithAssetSpecification("Full-size, Wireless 2.4GHz, USB Receiver, Battery Life 24 months")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000072"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Keyboard Dell KB216 Wired")
                    .WithAssetCode("KB000007")
                    .WithAssetSpecification("Full-size, USB, Membrane Keys, Spill-resistant, 104 Keys")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000073"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Keyboard HP 125 Wired")
                    .WithAssetCode("KB000008")
                    .WithAssetSpecification("Full-size, USB, Quiet Keys, Adjustable Tilt Legs, 104 Keys")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000074"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Keyboard Rapoo E9050G Wireless")
                    .WithAssetCode("KB000009")
                    .WithAssetSpecification("Slim, Wireless 2.4GHz, Multi-mode, USB Receiver, Scissor Switch")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000075"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Keyboard Genius KB-110X Wired")
                    .WithAssetCode("KB000010")
                    .WithAssetSpecification("Full-size, USB, Membrane, Spill-resistant, 104 Keys")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // MOUSE (MS)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000076"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Mouse Logitech M100 Wired")
                    .WithAssetCode("MS000006")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Ambidextrous, Plug-and-Play")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000077"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Mouse Dell MS116 Wired")
                    .WithAssetCode("MS000007")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Right-handed, Scroll Wheel")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000078"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Mouse HP X500 Wired")
                    .WithAssetCode("MS000008")
                    .WithAssetSpecification("USB, Optical, 800/1200/1600 DPI, Ergonomic, Scroll Wheel")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000079"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Mouse Rapoo N1130 Wired")
                    .WithAssetCode("MS000009")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Ambidextrous, 3-Button Design")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000080"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Mouse Genius DX-110 Wired")
                    .WithAssetCode("MS000010")
                    .WithAssetSpecification("USB, Optical, 1000 DPI, Scroll Wheel, Plug-and-Play")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // BLUETOOTH MOUSE (BM)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000081"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Bluetooth Mouse Logitech M650 L")
                    .WithAssetCode("BM000006")
                    .WithAssetSpecification("Bluetooth 5.0, 400-4000 DPI, Silent Click, 24-Month Battery Life")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000082"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Bluetooth Mouse Microsoft Arc")
                    .WithAssetCode("BM000007")
                    .WithAssetSpecification("Bluetooth 5.0, BlueTrack, Foldable Design, 6-Month Battery Life")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000083"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Bluetooth Mouse HP 240 Pike Silver")
                    .WithAssetCode("BM000008")
                    .WithAssetSpecification("Bluetooth 5.1, 1600 DPI, Silent Click, 16-Month Battery Life")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000084"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Bluetooth Mouse Rapoo M650 Silent")
                    .WithAssetCode("BM000009")
                    .WithAssetSpecification("Bluetooth 3.0/5.0, 1600 DPI, Silent Buttons, 9-Month Battery Life")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000085"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Bluetooth Mouse Dell MS700")
                    .WithAssetCode("BM000010")
                    .WithAssetSpecification("Bluetooth 5.0, 1600 DPI, Ergonomic, 36-Month Battery Life")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // BATTERY MONITOR (BM1)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000086"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Battery Monitor Eaton 5E 1100i USB")
                    .WithAssetCode("BM1000006")
                    .WithAssetSpecification("1100VA/660W, USB, 4 Output Sockets, Protection Time 9 min at full load")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 28, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 28, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000087"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Battery Monitor APC Back-UPS BX1000M")
                    .WithAssetCode("BM1000007")
                    .WithAssetSpecification("1000VA/600W, USB, 8 Output Sockets, LCD Display, AVR")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000088"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Battery Monitor CyberPower CP900EPFCLCD")
                    .WithAssetCode("BM1000008")
                    .WithAssetSpecification("900VA/540W, USB, 8 Output Sockets, LCD Panel, Pure Sine Wave")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000089"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Battery Monitor Santak TG-BOX 850")
                    .WithAssetCode("BM1000009")
                    .WithAssetSpecification("850VA/510W, USB, 4 Output Sockets, AVR, Backup Time 9 min")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000090"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Battery Monitor Ares AR-800VA")
                    .WithAssetCode("BM1000010")
                    .WithAssetSpecification("800VA/480W, USB, 4 Output Sockets, AVR, Backup Time 7 min")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // PRINTER (PR)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000091"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Printer HP LaserJet Pro M404dn")
                    .WithAssetCode("PR000006")
                    .WithAssetSpecification("A4, Mono Laser, 38ppm, Duplex, USB, LAN, 250-Sheet Tray")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 30, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000092"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Printer Canon imageCLASS LBP6030")
                    .WithAssetCode("PR000007")
                    .WithAssetSpecification("A4, Mono Laser, 18ppm, USB, 150-Sheet Tray, Compact Design")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 14, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 14, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000093"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Printer Epson EcoTank L3210")
                    .WithAssetCode("PR000008")
                    .WithAssetSpecification("A4, Color Inkjet, 10ppm Black/5ppm Color, USB, Ink Tank System")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000094"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Printer Brother DCP-L2540DW")
                    .WithAssetCode("PR000009")
                    .WithAssetSpecification("A4, Mono Laser MFP, 30ppm, Duplex, WiFi, USB, LAN, 250-Sheet Tray")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000095"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Printer Samsung SL-M2020W")
                    .WithAssetCode("PR000010")
                    .WithAssetSpecification("A4, Mono Laser, 21ppm, WiFi, USB, 150-Sheet Tray")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // SCANNER (SC)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000096"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Scanner Canon DR-C225W II")
                    .WithAssetCode("SC000006")
                    .WithAssetSpecification("A4, 25ppm/50ipm, ADF 30-Sheet, WiFi, USB, Duplex, CIS Sensor")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000097"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Scanner Epson DS-530 II")
                    .WithAssetCode("SC000007")
                    .WithAssetSpecification("A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000098"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Scanner Fujitsu fi-7030")
                    .WithAssetCode("SC000008")
                    .WithAssetSpecification("A4, 27ppm/54ipm, ADF 50-Sheet, USB, Duplex, CCD Sensor, 600 DPI")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000099"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Scanner HP ScanJet Pro 2000 s2")
                    .WithAssetCode("SC000009")
                    .WithAssetSpecification("A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000100"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Scanner Brother ADS-1200")
                    .WithAssetCode("SC000010")
                    .WithAssetSpecification("A4, 25ppm/50ipm, ADF 20-Sheet, USB, Duplex, CIS Sensor, 600 DPI")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // PROJECTOR (PJ)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000101"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Projector Epson EB-X49")
                    .WithAssetCode("PJ000006")
                    .WithAssetSpecification("XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, Lamp 12000h")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000102"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Projector Benq MX550")
                    .WithAssetCode("PJ000007")
                    .WithAssetSpecification("XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, SmartEco Mode")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000103"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Projector ViewSonic PA503X")
                    .WithAssetCode("PJ000008")
                    .WithAssetSpecification("XGA 1024x768, 3800 Lumens, HDMI, USB, VGA, Lamp 15000h")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000104"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Projector Optoma X400LVe")
                    .WithAssetCode("PJ000009")
                    .WithAssetSpecification("XGA 1024x768, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h ECO")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000105"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Projector Acer X1526HK")
                    .WithAssetCode("PJ000010")
                    .WithAssetSpecification("FHD 1920x1080, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 19, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 19, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // TABLET (TB)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000106"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Tablet Samsung Galaxy Tab A8")
                    .WithAssetCode("TB000006")
                    .WithAssetSpecification("10.5\" TFT, Unisoc T618, 4GB RAM, 64GB, WiFi, Android 11")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000107"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Tablet Lenovo Tab M10 Plus Gen 3")
                    .WithAssetCode("TB000007")
                    .WithAssetSpecification("10.61\" 2K IPS, Snapdragon 680, 4GB RAM, 128GB, WiFi, Android 12")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000108"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Tablet Huawei MatePad 11.5")
                    .WithAssetCode("TB000008")
                    .WithAssetSpecification("11.5\" IPS 120Hz, Snapdragon 7 Gen 1, 8GB RAM, 128GB, WiFi")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000109"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Tablet Xiaomi Pad 6")
                    .WithAssetCode("TB000009")
                    .WithAssetSpecification("11\" IPS 144Hz, Snapdragon 870, 8GB RAM, 256GB, WiFi, MIUI 14")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000110"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Tablet Amazon Fire HD 10")
                    .WithAssetCode("TB000010")
                    .WithAssetSpecification("10.1\" FHD IPS, Octa-Core 2.0GHz, 3GB RAM, 32GB, WiFi, FireOS")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 23, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // DESKTOP COMPUTER (DC)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000111"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Desktop Dell OptiPlex 3000 MT")
                    .WithAssetCode("DC000006")
                    .WithAssetSpecification("Intel Core i5-12500, 8GB RAM, 256GB SSD, Intel UHD 770, Win 11 Pro")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000112"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Desktop HP EliteDesk 800 G9 SFF")
                    .WithAssetCode("DC000007")
                    .WithAssetSpecification("Intel Core i7-12700, 16GB RAM, 512GB SSD, Intel UHD 770, Win 11 Pro")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000113"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Desktop Lenovo ThinkCentre M70s Gen 3")
                    .WithAssetCode("DC000008")
                    .WithAssetSpecification("Intel Core i5-12400, 8GB RAM, 256GB SSD, Intel UHD 730, Win 11 Pro")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000114"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Desktop Asus ExpertCenter D5 SFF D500SC")
                    .WithAssetCode("DC000009")
                    .WithAssetSpecification("Intel Core i3-10105, 8GB RAM, 256GB SSD, Intel UHD 630, Win 11 Pro")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000115"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Desktop Acer Veriton M6680G")
                    .WithAssetCode("DC000010")
                    .WithAssetSpecification("Intel Core i5-10400, 8GB RAM, 256GB SSD, Intel UHD 630, Win 10 Pro")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // NETWORK SWITCH (NS)
                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000116"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Network Switch Cisco CBS110-16T")
                    .WithAssetCode("NS000006")
                    .WithAssetSpecification("16-Port Gigabit, Unmanaged, Desktop, Fanless, QoS, IEEE 802.3az")
                    .WithAssetState(AssetState.Available)
                    .WithInstalledDateAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000117"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Network Switch TP-Link TL-SG1024D")
                    .WithAssetCode("NS000007")
                    .WithAssetSpecification("24-Port Gigabit, Unmanaged, Desktop/Rack-mount, Fanless, IEEE 802.3az")
                    .WithAssetState(AssetState.NotAvailable)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000118"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Network Switch D-Link DGS-1016D")
                    .WithAssetCode("NS000008")
                    .WithAssetSpecification("16-Port Gigabit, Unmanaged, Desktop, Fanless, Auto MDI/MDIX")
                    .WithAssetState(AssetState.Assigned)
                    .WithInstalledDateAtUtc(new DateTime(2026, 3, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000119"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Network Switch Netgear GS308E")
                    .WithAssetCode("NS000009")
                    .WithAssetSpecification("8-Port Gigabit, Smart Managed, Desktop, QoS, VLAN, IGMP Snooping")
                    .WithAssetState(AssetState.WaitingForRecycling)
                    .WithInstalledDateAtUtc(new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),

                new AssetBuilder()
                    .WithId(Guid.Parse("a0000000-0000-0000-0000-000000000120"))
                    .WithCategoryId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithLocationId(Guid.Parse("22222222-2222-2222-2222-222222222222"))
                    .WithAssetName("Network Switch MikroTik CRS112-8G-4S-IN")
                    .WithAssetCode("NS000010")
                    .WithAssetSpecification("8-Port Gigabit + 4 SFP, Managed, Desktop, RouterOS/SwitchOS, PoE")
                    .WithAssetState(AssetState.Recycled)
                    .WithInstalledDateAtUtc(new DateTime(2026, 1, 29, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 29, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build()
                #endregion
            };
            #endregion

            if (!await dbContext.Assets.AnyAsync())
            {
                dbContext.Assets.AddRange(assets);
                await dbContext.SaveChangesAsync();
            }

            #region Assignment
            var assignments = new List<Assignment>
            {
                // [Ha Noi] Asset: LT000001 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000001"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000001"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000004"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: MN000001 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000002"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000006"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000005"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 5, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 5, 26, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: KB000001 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000003"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000011"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000006"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: MS000001 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000004"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000016"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000007"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: BM000001 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000005"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000021"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: BM1000001 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000006"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000026"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: PR000001 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000007"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000031"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: SC000001 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000008"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000036"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: LT000003 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000009"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000003"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(true)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: MN000003 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000010"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000008"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000013"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: KB000003 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000011"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000013"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000014"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(true)
                    .WithCreatedAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ha Noi] Asset: MS000003 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000012"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000018"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000015"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: LT000006 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000013"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000061"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000019"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 5, 23, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 5, 23, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: MN000006 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000014"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000066"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000020"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000017"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 5, 27, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 5, 27, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: KB000006 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000015"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000071"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000021"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000018"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 6, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 6, 2, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: MS000006 | WaitingForAcceptance
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000016"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000076"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000022"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 6, 9, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.WaitingForAcceptance)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 6, 9, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: BM000006 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000017"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000081"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000023"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: BM1000006 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000018"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000086"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000024"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000017"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: PR000006 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000019"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000091"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000025"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000018"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: SC000006 | Returned
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000020"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000096"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000026"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Returned)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: LT000008 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000021"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000063"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000027"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 7, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(true)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 7, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: MN000008 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000022"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000068"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000028"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000017"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 2, 27, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 27, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: KB000008 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000023"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000073"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000029"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000018"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(true)
                    .WithCreatedAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [Ho Chi Minh] Asset: MS000008 | Accepted
                new AssignmentBuilder()
                    .WithId(Guid.Parse("b0000000-0000-0000-0000-000000000024"))
                    .WithAssetId(Guid.Parse("a0000000-0000-0000-0000-000000000078"))
                    .WithAssignedToUserId(Guid.Parse("10000000-0000-0000-0000-000000000030"))
                    .WithAssignedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithAssignedDateAtUtc(new DateTime(2026, 4, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithState(AssignmentState.Accepted)
                    .WithIsReturning(false)
                    .WithCreatedAtUtc(new DateTime(2026, 4, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build()

            };
            #endregion

            if (!await dbContext.Assignments.AnyAsync())
            {          
                await dbContext.Assignments.AddRangeAsync(assignments);
                await dbContext.SaveChangesAsync();
            }

            #region ReturnRequest
            var returnRequests = new List<ReturnRequest>
            {
                // [HN] Assignment: 009 | Asset: MN000003 | WaitingForReturning
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000001"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000009"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000012"))
                    .WithAcceptedByUserId(null)
                    .WithState(ReturnRequestState.WaitingForReturning)
                    .WithReturnedAtUtc(null)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 5, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [HN] Assignment: 011 | Asset: MS000003 | WaitingForReturning
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000002"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000011"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000014"))
                    .WithAcceptedByUserId(null)
                    .WithState(ReturnRequestState.WaitingForReturning)
                    .WithReturnedAtUtc(null)
                    .WithCreatedAtUtc(new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [HCM] Assignment: 021 | Asset: MN000008 | WaitingForReturning
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000003"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000021"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000027"))
                    .WithAcceptedByUserId(null)
                    .WithState(ReturnRequestState.WaitingForReturning)
                    .WithReturnedAtUtc(null)
                    .WithCreatedAtUtc(new DateTime(2026, 2, 7, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [HCM] Assignment: 023 | Asset: MS000008 | WaitingForReturning
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000004"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000023"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000029"))
                    .WithAcceptedByUserId(null)
                    .WithState(ReturnRequestState.WaitingForReturning)
                    .WithReturnedAtUtc(null)
                    .WithCreatedAtUtc(new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(null)
                    .Build(),
 
                // [HN] Assignment: 005 | Asset: BM1000001 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000005"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000005"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000008"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc))
                    .Build(),
 
                // [HN] Assignment: 006 | Asset: PR000001 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000006"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000006"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000009"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000002"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 2, 8, 0, 0, 0, DateTimeKind.Utc))
                    .Build(),
 
                // [HN] Assignment: 007 | Asset: SC000001 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000007"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000007"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000010"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000003"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 2, 27, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 2, 27, 0, 0, 0, DateTimeKind.Utc))
                    .Build(),
 
                // [HN] Assignment: 008 | Asset: LT000003 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000008"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000008"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000011"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000001"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 3, 17, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 3, 17, 0, 0, 0, DateTimeKind.Utc))
                    .Build(),
 
                // [HCM] Assignment: 017 | Asset: BM1000006 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000009"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000017"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000023"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 1, 25, 0, 0, 0, DateTimeKind.Utc))
                    .Build(),
 
                // [HCM] Assignment: 018 | Asset: PR000006 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000010"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000018"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000024"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000017"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc))
                    .Build(),
 
                // [HCM] Assignment: 019 | Asset: SC000006 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000011"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000019"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000025"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000018"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 2, 22, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc))
                    .Build(),
 
                // [HCM] Assignment: 020 | Asset: LT000008 | Completed
                new ReturnRequestBuilder()
                    .WithId(Guid.Parse("c0000000-0000-0000-0000-000000000012"))
                    .WithAssignmentId(Guid.Parse("b0000000-0000-0000-0000-000000000020"))
                    .WithRequestedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000026"))
                    .WithAcceptedByUserId(Guid.Parse("10000000-0000-0000-0000-000000000016"))
                    .WithState(ReturnRequestState.Completed)
                    .WithReturnedAtUtc(new DateTime(2026, 3, 19, 0, 0, 0, DateTimeKind.Utc))
                    .WithCreatedAtUtc(new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc))
                    .WithUpdatedAtUtc(new DateTime(2026, 3, 19, 0, 0, 0, DateTimeKind.Utc))
                    .Build()

            };
            #endregion

            if (!await dbContext.ReturnRequests.AnyAsync())
            {
                dbContext.ChangeTracker.Clear();
                await dbContext.ReturnRequests.AddRangeAsync(returnRequests);
                await dbContext.SaveChangesAsync();
            }

            return;
        }
    }
}
