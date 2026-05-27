using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Persistence.SeedData
{
    public static class SeedUserData
    {
        private static readonly Guid HaNoiId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid HcmId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static IReadOnlyList<(Guid Id, string UserName, string StaffCode, string FirstName, string LastName, Gender Gender, DateTime Dob, DateTime JoinedDate, Guid LocationId, UserType UserType)> GetData()
            => new List<(Guid, string, string, string, string, Gender, DateTime, DateTime, Guid, UserType)>
        {
            // HA NOI ADMINS
            (Guid.Parse("10000000-0000-0000-0000-000000000001"), "binhnv",  "SD0001", "Binh",  "Nguyen Van", Gender.Male,   new DateTime(1993, 01, 20), new DateTime(2018, 01, 20), HaNoiId, UserType.Admin),
            (Guid.Parse("10000000-0000-0000-0000-000000000002"), "huongtt", "SD0002", "Huong", "Tran Thi",   Gender.Female, new DateTime(1994, 03, 15), new DateTime(2019, 04, 10), HaNoiId, UserType.Admin),
            (Guid.Parse("10000000-0000-0000-0000-000000000003"), "datlq",   "SD0003", "Dat",   "Le Quang",   Gender.Male,   new DateTime(1992, 07, 11), new DateTime(2017, 08, 01), HaNoiId, UserType.Admin),
 
            // HA NOI STAFF
            (Guid.Parse("10000000-0000-0000-0000-000000000004"), "linhpt",  "SD0004", "Linh",  "Pham Thi",    Gender.Female, new DateTime(1998, 02, 18), new DateTime(2021, 03, 10), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000005"), "khanhdm", "SD0005", "Khanh", "Do Minh",     Gender.Male,   new DateTime(1997, 09, 22), new DateTime(2020, 10, 15), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000006"), "maivt",   "SD0006", "Mai",   "Vo Thi",      Gender.Female, new DateTime(1996, 12, 09), new DateTime(2019, 12, 20), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000007"), "tuanba",  "SD0007", "Tuan",  "Bui Anh",     Gender.Male,   new DateTime(1995, 06, 30), new DateTime(2018, 07, 15), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000008"), "ngandt",  "SD0008", "Ngan",  "Dang Thu",    Gender.Female, new DateTime(1999, 08, 14), new DateTime(2022, 01, 05), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000009"), "phonghg", "SD0009", "Phong", "Hoang Gia",   Gender.Male,   new DateTime(1991, 11, 25), new DateTime(2016, 05, 22), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000010"), "vybn",    "SD0010", "Vy",    "Bao Ngo",     Gender.Female, new DateTime(1998, 05, 17), new DateTime(2021, 09, 12), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000011"), "hungpd",  "SD0011", "Hung",  "Phan Duc",    Gender.Male,   new DateTime(1990, 04, 02), new DateTime(2015, 04, 20), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000012"), "thaoln",  "SD0012", "Thao",  "Ly Ngoc",     Gender.Female, new DateTime(1997, 01, 28), new DateTime(2020, 02, 11), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000013"), "quantm",  "SD0013", "Quan",  "Truong Minh", Gender.Male,   new DateTime(1993, 10, 13), new DateTime(2018, 11, 01), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000014"), "yendt",   "SD0014", "Yen",   "Duong Thanh", Gender.Female, new DateTime(1996, 07, 07), new DateTime(2019, 08, 18), HaNoiId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000015"), "namct",   "SD0015", "Nam",   "Cao Tuan",    Gender.Male,   new DateTime(1994, 09, 19), new DateTime(2019, 10, 05), HaNoiId, UserType.Staff),
 
            // HCM ADMINS
            (Guid.Parse("10000000-0000-0000-0000-000000000016"), "annh",    "SD0016", "An",    "Nguyen Hoang", Gender.Male,   new DateTime(1992, 04, 12), new DateTime(2017, 06, 15), HcmId, UserType.Admin),
            (Guid.Parse("10000000-0000-0000-0000-000000000017"), "trangpn", "SD0017", "Trang", "Pham Ngoc",    Gender.Female, new DateTime(1995, 02, 24), new DateTime(2019, 03, 08), HcmId, UserType.Admin),
            (Guid.Parse("10000000-0000-0000-0000-000000000018"), "kiettm",  "SD0018", "Kiet",  "Tran Minh",    Gender.Male,   new DateTime(1991, 08, 06), new DateTime(2016, 09, 01), HcmId, UserType.Admin),
 
            // HCM STAFF
            (Guid.Parse("10000000-0000-0000-0000-000000000019"), "lanlt",   "SD0019", "Lan",   "Le Thi",     Gender.Female, new DateTime(1997, 03, 09), new DateTime(2020, 04, 12), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000020"), "hieuvt",  "SD0020", "Hieu",  "Vo Thanh",   Gender.Male,   new DateTime(1996, 12, 21), new DateTime(2020, 01, 17), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000021"), "mydb",    "SD0021", "My",    "Dang Bao",   Gender.Female, new DateTime(1998, 06, 11), new DateTime(2021, 07, 22), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000022"), "ducbg",   "SD0022", "Duc",   "Bui Gia",    Gender.Male,   new DateTime(1993, 05, 28), new DateTime(2018, 06, 30), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000023"), "nhiht",   "SD0023", "Nhi",   "Ho Thi",     Gender.Female, new DateTime(1999, 09, 14), new DateTime(2022, 02, 15), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000024"), "taipa",   "SD0024", "Tai",   "Phan Anh",   Gender.Male,   new DateTime(1990, 01, 03), new DateTime(2015, 03, 10), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000025"), "chaunm",  "SD0025", "Chau",  "Ngo Minh",   Gender.Female, new DateTime(1997, 07, 18), new DateTime(2020, 09, 09), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000026"), "vietlq",  "SD0026", "Viet",  "Ly Quoc",    Gender.Male,   new DateTime(1994, 11, 29), new DateTime(2019, 01, 15), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000027"), "hanhdk",  "SD0027", "Hanh",  "Duong Kim",  Gender.Female, new DateTime(1995, 10, 10), new DateTime(2019, 11, 11), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000028"), "sontd",   "SD0028", "Son",   "Trinh Duc",  Gender.Male,   new DateTime(1992, 02, 16), new DateTime(2017, 03, 18), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000029"), "ngamt",   "SD0029", "Nga",   "Mai Thu",    Gender.Female, new DateTime(1998, 08, 27), new DateTime(2021, 10, 20), HcmId, UserType.Staff),
            (Guid.Parse("10000000-0000-0000-0000-000000000030"), "longcv",  "SD0030", "Long",  "Cao Van",    Gender.Male,   new DateTime(1993, 12, 05), new DateTime(2018, 12, 01), HcmId, UserType.Staff),
        };
    }
}
