using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.Persistence.Builder;

namespace NashAssetManagement.Persistence.SeedData
{
    public static class SeedAssetData
    {
        private static readonly Guid HaNoiLocationId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid HcmLocationId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid CatLaptop = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private static readonly Guid CatMonitor = Guid.Parse("10000000-0000-0000-0000-000000000002");
        private static readonly Guid CatKeyboard = Guid.Parse("10000000-0000-0000-0000-000000000003");
        private static readonly Guid CatMouse = Guid.Parse("10000000-0000-0000-0000-000000000004");
        private static readonly Guid CatBluetoothMouse = Guid.Parse("10000000-0000-0000-0000-000000000005");
        private static readonly Guid CatBatteryMonitor = Guid.Parse("10000000-0000-0000-0000-000000000006");
        private static readonly Guid CatPrinter = Guid.Parse("10000000-0000-0000-0000-000000000007");
        private static readonly Guid CatScanner = Guid.Parse("10000000-0000-0000-0000-000000000008");
        private static readonly Guid CatProjector = Guid.Parse("10000000-0000-0000-0000-000000000009");
        private static readonly Guid CatTablet = Guid.Parse("10000000-0000-0000-0000-000000000010");
        private static readonly Guid CatDesktop = Guid.Parse("10000000-0000-0000-0000-000000000011");
        private static readonly Guid CatNetworkSwitch = Guid.Parse("10000000-0000-0000-0000-000000000012");

        public static List<Asset> GetData()
        {
            var assets = new List<Asset>();
            assets.AddRange(GetHaNoiAssets());
            assets.AddRange(GetHcmAssets());
            return assets;
        }

        #region Ha Noi Assets
        private static IEnumerable<Asset> GetHaNoiAssets()
        {
            return new List<Asset>
            {
                // LAPTOP (LT)
                Build("a0000000-0000-0000-0000-000000000001", CatLaptop, HaNoiLocationId, "Laptop HP ProBook 450 G9",          "LT000001", "Intel Core i5-1235U, 8GB RAM, 256GB SSD, 15.6\" FHD",                              AssetState.Available,          new DateTime(2026, 1, 10)),
                Build("a0000000-0000-0000-0000-000000000002", CatLaptop, HaNoiLocationId, "Laptop Dell Latitude 5540",          "LT000002", "Intel Core i7-1365U, 16GB RAM, 512GB SSD, 15.6\" FHD",                             AssetState.NotAvailable,       new DateTime(2026, 1, 20)),
                Build("a0000000-0000-0000-0000-000000000003", CatLaptop, HaNoiLocationId, "Laptop Lenovo ThinkPad E15 Gen 4",   "LT000003", "AMD Ryzen 5 5625U, 16GB RAM, 512GB SSD, 15.6\" FHD",                               AssetState.Assigned,           new DateTime(2026, 2,  5)),  
                Build("a0000000-0000-0000-0000-000000000004", CatLaptop, HaNoiLocationId, "Laptop Asus VivoBook 15 X1502",      "LT000004", "Intel Core i5-12500H, 8GB RAM, 512GB SSD, 15.6\" FHD",                             AssetState.WaitingForRecycling, new DateTime(2026, 2, 15)),
                Build("a0000000-0000-0000-0000-000000000005", CatLaptop, HaNoiLocationId, "Laptop Acer Aspire 5 A515-57",       "LT000005", "Intel Core i5-12450H, 8GB RAM, 512GB SSD, 15.6\" FHD",                             AssetState.Recycled,           new DateTime(2026, 1,  5)),
 
                // MONITOR (MN)
                Build("a0000000-0000-0000-0000-000000000006", CatMonitor, HaNoiLocationId, "Monitor Dell UltraSharp U2422H",   "MN000001", "24\" FHD IPS, 1920x1080, 60Hz, HDMI, DisplayPort, USB-C",                          AssetState.Available,          new DateTime(2026, 1, 12)),
                Build("a0000000-0000-0000-0000-000000000007", CatMonitor, HaNoiLocationId, "Monitor LG 27UK850-W",             "MN000002", "27\" 4K UHD IPS, 3840x2160, 60Hz, HDMI, DisplayPort, USB-C",                       AssetState.NotAvailable,       new DateTime(2026, 1, 22)),
                Build("a0000000-0000-0000-0000-000000000008", CatMonitor, HaNoiLocationId, "Monitor Samsung F24T450FQE",       "MN000003", "24\" FHD IPS, 1920x1080, 75Hz, HDMI, DisplayPort, Adjustable Stand",               AssetState.Assigned,           new DateTime(2026, 2,  8)),
                Build("a0000000-0000-0000-0000-000000000009", CatMonitor, HaNoiLocationId, "Monitor BenQ GW2480",              "MN000004", "24\" FHD IPS, 1920x1080, 60Hz, HDMI, VGA, Eye-Care Technology",                    AssetState.WaitingForRecycling, new DateTime(2026, 3,  1)),
                Build("a0000000-0000-0000-0000-000000000010", CatMonitor, HaNoiLocationId, "Monitor AOC 22B2HM",               "MN000005", "21.5\" FHD VA, 1920x1080, 75Hz, HDMI, VGA, Flicker-Free",                          AssetState.Recycled,           new DateTime(2026, 1,  7)),
 
                // KEYBOARD (KB)
                Build("a0000000-0000-0000-0000-000000000011", CatKeyboard, HaNoiLocationId, "Keyboard Logitech MK270 Wireless", "KB000001", "Full-size, Wireless 2.4GHz, USB Receiver, Battery Life 24 months",               AssetState.Available,          new DateTime(2026, 1, 15)),
                Build("a0000000-0000-0000-0000-000000000012", CatKeyboard, HaNoiLocationId, "Keyboard Dell KB216 Wired",        "KB000002", "Full-size, USB, Membrane Keys, Spill-resistant, 104 Keys",                        AssetState.NotAvailable,       new DateTime(2026, 2,  1)),
                Build("a0000000-0000-0000-0000-000000000013", CatKeyboard, HaNoiLocationId, "Keyboard HP 125 Wired",            "KB000003", "Full-size, USB, Quiet Keys, Adjustable Tilt Legs, 104 Keys",                      AssetState.Assigned,           new DateTime(2026, 2, 20)),
                Build("a0000000-0000-0000-0000-000000000014", CatKeyboard, HaNoiLocationId, "Keyboard Rapoo E9050G Wireless",   "KB000004", "Slim, Wireless 2.4GHz, Multi-mode, USB Receiver, Scissor Switch",                 AssetState.WaitingForRecycling, new DateTime(2026, 3,  5)),
                Build("a0000000-0000-0000-0000-000000000015", CatKeyboard, HaNoiLocationId, "Keyboard Genius KB-110X Wired",    "KB000005", "Full-size, USB, Membrane, Spill-resistant, 104 Keys",                             AssetState.Recycled,           new DateTime(2026, 1,  8)),
 
                // MOUSE (MS)
                Build("a0000000-0000-0000-0000-000000000016", CatMouse, HaNoiLocationId, "Mouse Logitech M100 Wired",          "MS000001", "USB, Optical, 1000 DPI, Ambidextrous, Plug-and-Play",                             AssetState.Available,          new DateTime(2026, 1, 18)),
                Build("a0000000-0000-0000-0000-000000000017", CatMouse, HaNoiLocationId, "Mouse Dell MS116 Wired",             "MS000002", "USB, Optical, 1000 DPI, Right-handed, Scroll Wheel",                              AssetState.NotAvailable,       new DateTime(2026, 2,  3)),
                Build("a0000000-0000-0000-0000-000000000018", CatMouse, HaNoiLocationId, "Mouse HP X500 Wired",                "MS000003", "USB, Optical, 800/1200/1600 DPI, Ergonomic, Scroll Wheel",                        AssetState.Assigned,           new DateTime(2026, 2, 25)),
                Build("a0000000-0000-0000-0000-000000000019", CatMouse, HaNoiLocationId, "Mouse Rapoo N1130 Wired",            "MS000004", "USB, Optical, 1000 DPI, Ambidextrous, 3-Button Design",                           AssetState.WaitingForRecycling, new DateTime(2026, 3, 10)),
                Build("a0000000-0000-0000-0000-000000000020", CatMouse, HaNoiLocationId, "Mouse Genius DX-110 Wired",          "MS000005", "USB, Optical, 1000 DPI, Scroll Wheel, Plug-and-Play",                             AssetState.Recycled,           new DateTime(2026, 1,  6)),
 
                // BLUETOOTH MOUSE (BM)
                Build("a0000000-0000-0000-0000-000000000021", CatBluetoothMouse, HaNoiLocationId, "Bluetooth Mouse Logitech M650 L",   "BM000001", "Bluetooth 5.0, 400-4000 DPI, Silent Click, 24-Month Battery Life",        AssetState.Available,          new DateTime(2026, 1, 25)),
                Build("a0000000-0000-0000-0000-000000000022", CatBluetoothMouse, HaNoiLocationId, "Bluetooth Mouse Microsoft Arc",      "BM000002", "Bluetooth 5.0, BlueTrack, Foldable Design, 6-Month Battery Life",         AssetState.NotAvailable,       new DateTime(2026, 2, 10)),
                Build("a0000000-0000-0000-0000-000000000023", CatBluetoothMouse, HaNoiLocationId, "Bluetooth Mouse HP 240 Pike Silver", "BM000003", "Bluetooth 5.1, 1600 DPI, Silent Click, 16-Month Battery Life",            AssetState.Assigned,           new DateTime(2026, 3,  2)),
                Build("a0000000-0000-0000-0000-000000000024", CatBluetoothMouse, HaNoiLocationId, "Bluetooth Mouse Rapoo M650 Silent",  "BM000004", "Bluetooth 3.0/5.0, 1600 DPI, Silent Buttons, 9-Month Battery Life",       AssetState.WaitingForRecycling, new DateTime(2026, 3, 15)),
                Build("a0000000-0000-0000-0000-000000000025", CatBluetoothMouse, HaNoiLocationId, "Bluetooth Mouse Dell MS700",         "BM000005", "Bluetooth 5.0, 1600 DPI, Ergonomic, 36-Month Battery Life",               AssetState.Recycled,           new DateTime(2026, 1,  9)),
 
                // BATTERY MONITOR (BM1)
                Build("a0000000-0000-0000-0000-000000000026", CatBatteryMonitor, HaNoiLocationId, "Battery Monitor Eaton 5E 1100i USB",       "BM1000001", "1100VA/660W, USB, 4 Output Sockets, Protection Time 9 min at full load", AssetState.Available,          new DateTime(2026, 1, 28)),
                Build("a0000000-0000-0000-0000-000000000027", CatBatteryMonitor, HaNoiLocationId, "Battery Monitor APC Back-UPS BX1000M",     "BM1000002", "1000VA/600W, USB, 8 Output Sockets, LCD Display, AVR",                   AssetState.NotAvailable,       new DateTime(2026, 2, 12)),
                Build("a0000000-0000-0000-0000-000000000028", CatBatteryMonitor, HaNoiLocationId, "Battery Monitor CyberPower CP900EPFCLCD",  "BM1000003", "900VA/540W, USB, 8 Output Sockets, LCD Panel, Pure Sine Wave",            AssetState.Assigned,           new DateTime(2026, 3,  5)),
                Build("a0000000-0000-0000-0000-000000000029", CatBatteryMonitor, HaNoiLocationId, "Battery Monitor Santak TG-BOX 850",        "BM1000004", "850VA/510W, USB, 4 Output Sockets, AVR, Backup Time 9 min",               AssetState.WaitingForRecycling, new DateTime(2026, 3, 18)),
                Build("a0000000-0000-0000-0000-000000000030", CatBatteryMonitor, HaNoiLocationId, "Battery Monitor Ares AR-800VA",            "BM1000005", "800VA/480W, USB, 4 Output Sockets, AVR, Backup Time 7 min",               AssetState.Recycled,           new DateTime(2026, 1, 11)),
 
                // PRINTER (PR)
                Build("a0000000-0000-0000-0000-000000000031", CatPrinter, HaNoiLocationId, "Printer HP LaserJet Pro M404dn",      "PR000001", "A4, Mono Laser, 38ppm, Duplex, USB, LAN, 250-Sheet Tray",                         AssetState.Available,          new DateTime(2026, 1, 30)),
                Build("a0000000-0000-0000-0000-000000000032", CatPrinter, HaNoiLocationId, "Printer Canon imageCLASS LBP6030",    "PR000002", "A4, Mono Laser, 18ppm, USB, 150-Sheet Tray, Compact Design",                      AssetState.NotAvailable,       new DateTime(2026, 2, 14)),
                Build("a0000000-0000-0000-0000-000000000033", CatPrinter, HaNoiLocationId, "Printer Epson EcoTank L3210",         "PR000003", "A4, Color Inkjet, 10ppm Black/5ppm Color, USB, Ink Tank System",                   AssetState.Assigned,           new DateTime(2026, 3,  8)),
                Build("a0000000-0000-0000-0000-000000000034", CatPrinter, HaNoiLocationId, "Printer Brother DCP-L2540DW",         "PR000004", "A4, Mono Laser MFP, 30ppm, Duplex, WiFi, USB, LAN, 250-Sheet Tray",               AssetState.WaitingForRecycling, new DateTime(2026, 3, 20)),
                Build("a0000000-0000-0000-0000-000000000035", CatPrinter, HaNoiLocationId, "Printer Samsung SL-M2020W",           "PR000005", "A4, Mono Laser, 21ppm, WiFi, USB, 150-Sheet Tray",                                AssetState.Recycled,           new DateTime(2026, 1, 13)),
 
                // SCANNER (SC)
                Build("a0000000-0000-0000-0000-000000000036", CatScanner, HaNoiLocationId, "Scanner Canon DR-C225W II",           "SC000001", "A4, 25ppm/50ipm, ADF 30-Sheet, WiFi, USB, Duplex, CIS Sensor",                    AssetState.Available,          new DateTime(2026, 2,  2)),
                Build("a0000000-0000-0000-0000-000000000037", CatScanner, HaNoiLocationId, "Scanner Epson DS-530 II",             "SC000002", "A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI",                 AssetState.NotAvailable,       new DateTime(2026, 2, 18)),
                Build("a0000000-0000-0000-0000-000000000038", CatScanner, HaNoiLocationId, "Scanner Fujitsu fi-7030",             "SC000003", "A4, 27ppm/54ipm, ADF 50-Sheet, USB, Duplex, CCD Sensor, 600 DPI",                 AssetState.Assigned,           new DateTime(2026, 3, 12)),
                Build("a0000000-0000-0000-0000-000000000039", CatScanner, HaNoiLocationId, "Scanner HP ScanJet Pro 2000 s2",      "SC000004", "A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI",                 AssetState.WaitingForRecycling, new DateTime(2026, 3, 22)),
                Build("a0000000-0000-0000-0000-000000000040", CatScanner, HaNoiLocationId, "Scanner Brother ADS-1200",            "SC000005", "A4, 25ppm/50ipm, ADF 20-Sheet, USB, Duplex, CIS Sensor, 600 DPI",                 AssetState.Recycled,           new DateTime(2026, 1, 16)),
 
                // PROJECTOR (PJ)
                Build("a0000000-0000-0000-0000-000000000041", CatProjector, HaNoiLocationId, "Projector Epson EB-X49",            "PJ000001", "XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, Lamp 12000h",                          AssetState.Available,          new DateTime(2026, 2,  5)),
                Build("a0000000-0000-0000-0000-000000000042", CatProjector, HaNoiLocationId, "Projector Benq MX550",              "PJ000002", "XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, SmartEco Mode",                        AssetState.NotAvailable,       new DateTime(2026, 2, 22)),
                Build("a0000000-0000-0000-0000-000000000043", CatProjector, HaNoiLocationId, "Projector ViewSonic PA503X",        "PJ000003", "XGA 1024x768, 3800 Lumens, HDMI, USB, VGA, Lamp 15000h",                          AssetState.Assigned,           new DateTime(2026, 3, 15)),
                Build("a0000000-0000-0000-0000-000000000044", CatProjector, HaNoiLocationId, "Projector Optoma X400LVe",          "PJ000004", "XGA 1024x768, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h ECO",                      AssetState.WaitingForRecycling, new DateTime(2026, 4,  1)),
                Build("a0000000-0000-0000-0000-000000000045", CatProjector, HaNoiLocationId, "Projector Acer X1526HK",            "PJ000005", "FHD 1920x1080, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h",                         AssetState.Recycled,           new DateTime(2026, 1, 19)),
 
                // TABLET (TB)
                Build("a0000000-0000-0000-0000-000000000046", CatTablet, HaNoiLocationId, "Tablet Samsung Galaxy Tab A8",         "TB000001", "10.5\" TFT, Unisoc T618, 4GB RAM, 64GB, WiFi, Android 11",                        AssetState.Available,          new DateTime(2026, 2,  8)),
                Build("a0000000-0000-0000-0000-000000000047", CatTablet, HaNoiLocationId, "Tablet Lenovo Tab M10 Plus Gen 3",     "TB000002", "10.61\" 2K IPS, Snapdragon 680, 4GB RAM, 128GB, WiFi, Android 12",                AssetState.NotAvailable,       new DateTime(2026, 2, 26)),
                Build("a0000000-0000-0000-0000-000000000048", CatTablet, HaNoiLocationId, "Tablet Huawei MatePad 11.5",           "TB000003", "11.5\" IPS 120Hz, Snapdragon 7 Gen 1, 8GB RAM, 128GB, WiFi",                      AssetState.Assigned,           new DateTime(2026, 3, 18)),
                Build("a0000000-0000-0000-0000-000000000049", CatTablet, HaNoiLocationId, "Tablet Xiaomi Pad 6",                  "TB000004", "11\" IPS 144Hz, Snapdragon 870, 8GB RAM, 256GB, WiFi, MIUI 14",                   AssetState.WaitingForRecycling, new DateTime(2026, 4,  5)),
                Build("a0000000-0000-0000-0000-000000000050", CatTablet, HaNoiLocationId, "Tablet Amazon Fire HD 10",             "TB000005", "10.1\" FHD IPS, Octa-Core 2.0GHz, 3GB RAM, 32GB, WiFi, FireOS",                   AssetState.Recycled,           new DateTime(2026, 1, 23)),
 
                // DESKTOP COMPUTER (DC)
                Build("a0000000-0000-0000-0000-000000000051", CatDesktop, HaNoiLocationId, "Desktop Dell OptiPlex 3000 MT",        "DC000001", "Intel Core i5-12500, 8GB RAM, 256GB SSD, Intel UHD 770, Win 11 Pro",             AssetState.Available,          new DateTime(2026, 2, 10)),
                Build("a0000000-0000-0000-0000-000000000052", CatDesktop, HaNoiLocationId, "Desktop HP EliteDesk 800 G9 SFF",      "DC000002", "Intel Core i7-12700, 16GB RAM, 512GB SSD, Intel UHD 770, Win 11 Pro",            AssetState.NotAvailable,       new DateTime(2026, 3,  1)),
                Build("a0000000-0000-0000-0000-000000000053", CatDesktop, HaNoiLocationId, "Desktop Lenovo ThinkCentre M70s Gen 3","DC000003", "Intel Core i5-12400, 8GB RAM, 256GB SSD, Intel UHD 730, Win 11 Pro",             AssetState.Assigned,           new DateTime(2026, 3, 20)),
                Build("a0000000-0000-0000-0000-000000000054", CatDesktop, HaNoiLocationId, "Desktop Asus ExpertCenter D5 SFF D500SC","DC000004","Intel Core i3-10105, 8GB RAM, 256GB SSD, Intel UHD 630, Win 11 Pro",            AssetState.WaitingForRecycling, new DateTime(2026, 4,  8)),
                Build("a0000000-0000-0000-0000-000000000055", CatDesktop, HaNoiLocationId, "Desktop Acer Veriton M6680G",          "DC000005", "Intel Core i5-10400, 8GB RAM, 256GB SSD, Intel UHD 630, Win 10 Pro",             AssetState.Recycled,           new DateTime(2026, 1, 26)),
 
                // NETWORK SWITCH (NS)
                Build("a0000000-0000-0000-0000-000000000056", CatNetworkSwitch, HaNoiLocationId, "Network Switch Cisco CBS110-16T",          "NS000001", "16-Port Gigabit, Unmanaged, Desktop, Fanless, QoS, IEEE 802.3az",           AssetState.Available,          new DateTime(2026, 2, 15)),
                Build("a0000000-0000-0000-0000-000000000057", CatNetworkSwitch, HaNoiLocationId, "Network Switch TP-Link TL-SG1024D",        "NS000002", "24-Port Gigabit, Unmanaged, Desktop/Rack-mount, Fanless, IEEE 802.3az",     AssetState.NotAvailable,       new DateTime(2026, 3,  3)),
                Build("a0000000-0000-0000-0000-000000000058", CatNetworkSwitch, HaNoiLocationId, "Network Switch D-Link DGS-1016D",          "NS000003", "16-Port Gigabit, Unmanaged, Desktop, Fanless, Auto MDI/MDIX",               AssetState.Assigned,           new DateTime(2026, 3, 25)),
                Build("a0000000-0000-0000-0000-000000000059", CatNetworkSwitch, HaNoiLocationId, "Network Switch Netgear GS308E",            "NS000004", "8-Port Gigabit, Smart Managed, Desktop, QoS, VLAN, IGMP Snooping",          AssetState.WaitingForRecycling, new DateTime(2026, 4, 10)),
                Build("a0000000-0000-0000-0000-000000000060", CatNetworkSwitch, HaNoiLocationId, "Network Switch MikroTik CRS112-8G-4S-IN",  "NS000005", "8-Port Gigabit + 4 SFP, Managed, Desktop, RouterOS/SwitchOS, PoE",          AssetState.Recycled,           new DateTime(2026, 1, 29)),

                Build("a0000000-0000-0000-0000-000000000061", CatLaptop, HaNoiLocationId, "Laptop MSI Modern 14",           "LT000006", "Intel Core i5-1335U, 16GB RAM, 512GB SSD, 14\" FHD",                    AssetState.Available,           new DateTime(2026, 4, 11)),
                Build("a0000000-0000-0000-0000-000000000062", CatLaptop, HaNoiLocationId, "Laptop Lenovo IdeaPad Slim 5", "LT000007", "AMD Ryzen 7 7730U, 16GB RAM, 512GB SSD, 15.6\" FHD",                   AssetState.Assigned,            new DateTime(2026, 3, 21)),
                Build("a0000000-0000-0000-0000-000000000063", CatLaptop, HaNoiLocationId, "Laptop Dell Inspiron 3530",    "LT000008", "Intel Core i5-1235U, 8GB RAM, 512GB SSD, 15.6\" FHD",                   AssetState.NotAvailable,        new DateTime(2026, 2, 13)),
                Build("a0000000-0000-0000-0000-000000000064", CatLaptop, HaNoiLocationId, "Laptop Asus Zenbook 14",       "LT000009", "Intel Core Ultra 5, 16GB RAM, 1TB SSD, 14\" OLED",                     AssetState.WaitingForRecycling, new DateTime(2026, 1, 18)),
                Build("a0000000-0000-0000-0000-000000000065", CatLaptop, HaNoiLocationId, "Laptop Acer Swift Go 14",      "LT000010", "Intel Core i7-1355U, 16GB RAM, 512GB SSD, 14\" IPS",                    AssetState.Recycled,            new DateTime(2026, 2, 28)),

                Build("a0000000-0000-0000-0000-000000000066", CatMonitor, HaNoiLocationId, "Monitor MSI PRO MP241X",      "MN000006", "23.8\" FHD VA, 75Hz, HDMI, VGA",                                        AssetState.Available,           new DateTime(2026, 4, 12)),
                Build("a0000000-0000-0000-0000-000000000067", CatMonitor, HaNoiLocationId, "Monitor Asus VA24EHF",        "MN000007", "23.8\" FHD IPS, 100Hz, HDMI",                                           AssetState.Assigned,            new DateTime(2026, 3, 19)),
                Build("a0000000-0000-0000-0000-000000000068", CatMonitor, HaNoiLocationId, "Monitor ViewSonic VA2432-H",  "MN000008", "24\" FHD IPS, 75Hz, HDMI, VGA",                                         AssetState.NotAvailable,        new DateTime(2026, 2, 22)),
                Build("a0000000-0000-0000-0000-000000000069", CatMonitor, HaNoiLocationId, "Monitor LG 24MP400-B",        "MN000009", "24\" FHD IPS, 75Hz, HDMI",                                              AssetState.WaitingForRecycling, new DateTime(2026, 1, 25)),
                Build("a0000000-0000-0000-0000-000000000070", CatMonitor, HaNoiLocationId, "Monitor Philips 242E1GSJ",    "MN000010", "23.8\" FHD VA, 144Hz, HDMI",                                             AssetState.Recycled,            new DateTime(2026, 2, 8)),

                Build("a0000000-0000-0000-0000-000000000071", CatKeyboard, HaNoiLocationId, "Keyboard Logitech K120",     "KB000006", "USB, Full-size, Spill-resistant",                                        AssetState.Available,           new DateTime(2026, 4, 15)),
                Build("a0000000-0000-0000-0000-000000000072", CatKeyboard, HaNoiLocationId, "Keyboard DareU LK185",       "KB000007", "USB, Membrane, Full-size",                                               AssetState.Assigned,            new DateTime(2026, 3, 11)),
                Build("a0000000-0000-0000-0000-000000000073", CatKeyboard, HaNoiLocationId, "Keyboard Rapoo NK2600",      "KB000008", "USB, Spill-resistant, Silent Keys",                                       AssetState.NotAvailable,        new DateTime(2026, 2, 5)),
                Build("a0000000-0000-0000-0000-000000000074", CatKeyboard, HaNoiLocationId, "Keyboard HP K1500",          "KB000009", "USB, Ergonomic Design, 104 Keys",                                         AssetState.WaitingForRecycling, new DateTime(2026, 1, 20)),
                Build("a0000000-0000-0000-0000-000000000075", CatKeyboard, HaNoiLocationId, "Keyboard Asus Marshmallow",  "KB000010", "Wireless Bluetooth, Compact Layout",                                      AssetState.Recycled,            new DateTime(2026, 3, 2)),

                Build("a0000000-0000-0000-0000-000000000076", CatMouse, HaNoiLocationId, "Mouse Logitech B100",          "MS000006", "USB, Optical, 800 DPI",                                                   AssetState.Available,           new DateTime(2026, 4, 1)),
                Build("a0000000-0000-0000-0000-000000000077", CatMouse, HaNoiLocationId, "Mouse Asus UT280",             "MS000007", "USB, Optical, 1000 DPI",                                                  AssetState.Assigned,            new DateTime(2026, 3, 17)),
                Build("a0000000-0000-0000-0000-000000000078", CatMouse, HaNoiLocationId, "Mouse Rapoo N200",             "MS000008", "USB, Optical, 1600 DPI",                                                  AssetState.NotAvailable,        new DateTime(2026, 2, 16)),
                Build("a0000000-0000-0000-0000-000000000079", CatMouse, HaNoiLocationId, "Mouse Dell WM126 Wireless",    "MS000009", "Wireless 2.4GHz, Optical",                                                AssetState.WaitingForRecycling, new DateTime(2026, 1, 30)),
                Build("a0000000-0000-0000-0000-000000000080", CatMouse, HaNoiLocationId, "Mouse HP M10",                 "MS000010", "USB, Optical, Compact Design",                                             AssetState.Recycled,            new DateTime(2026, 3, 8)),

                Build("a0000000-0000-0000-0000-000000000081", CatPrinter, HaNoiLocationId, "Printer Canon G3010",        "PR000006", "A4, Color Ink Tank, WiFi, USB",                                            AssetState.Available,           new DateTime(2026, 4, 5)),
                Build("a0000000-0000-0000-0000-000000000082", CatPrinter, HaNoiLocationId, "Printer Brother HL-L2366DW", "PR000007", "A4, Mono Laser, Duplex, WiFi",                                             AssetState.Assigned,            new DateTime(2026, 3, 12)),
                Build("a0000000-0000-0000-0000-000000000083", CatPrinter, HaNoiLocationId, "Printer Epson L3250",        "PR000008", "A4, Color Ink Tank, WiFi",                                                 AssetState.NotAvailable,        new DateTime(2026, 2, 27)),
                Build("a0000000-0000-0000-0000-000000000084", CatPrinter, HaNoiLocationId, "Printer HP Smart Tank 580",  "PR000009", "A4, Color Ink Tank, Mobile Printing",                                      AssetState.WaitingForRecycling, new DateTime(2026, 1, 14)),
                Build("a0000000-0000-0000-0000-000000000085", CatPrinter, HaNoiLocationId, "Printer Pantum P2500W",      "PR000010", "A4, Mono Laser, WiFi, USB",                                                AssetState.Recycled,            new DateTime(2026, 2, 18)),

                Build("a0000000-0000-0000-0000-000000000086", CatDesktop, HaNoiLocationId, "Desktop HP Pro Tower 280 G9","DC000006", "Intel Core i5-12400, 8GB RAM, 512GB SSD",                                AssetState.Available,           new DateTime(2026, 4, 10)),
                Build("a0000000-0000-0000-0000-000000000087", CatDesktop, HaNoiLocationId, "Desktop Dell Vostro 3910",   "DC000007", "Intel Core i7-12700, 16GB RAM, 512GB SSD",                               AssetState.Assigned,            new DateTime(2026, 3, 22)),
                Build("a0000000-0000-0000-0000-000000000088", CatDesktop, HaNoiLocationId, "Desktop Lenovo Neo 50s",     "DC000008", "Intel Core i5-13400, 8GB RAM, 256GB SSD",                                AssetState.NotAvailable,        new DateTime(2026, 2, 7)),
                Build("a0000000-0000-0000-0000-000000000089", CatDesktop, HaNoiLocationId, "Desktop Asus S500SC",         "DC000009", "Intel Core i3-12100, 8GB RAM, 256GB SSD",                                AssetState.WaitingForRecycling, new DateTime(2026, 1, 27)),
                Build("a0000000-0000-0000-0000-000000000090", CatDesktop, HaNoiLocationId, "Desktop Acer Aspire TC",      "DC000010", "Intel Core i5-11400, 8GB RAM, 512GB SSD",                                AssetState.Recycled,            new DateTime(2026, 2, 11)),

                Build("a0000000-0000-0000-0000-000000000091", CatProjector, HaNoiLocationId, "Projector Epson CO-FH02",  "PJ000006", "FHD 1920x1080, 3000 Lumens, HDMI",                                          AssetState.Available,           new DateTime(2026, 4, 2)),
                Build("a0000000-0000-0000-0000-000000000092", CatProjector, HaNoiLocationId, "Projector BenQ MS560",      "PJ000007", "SVGA 800x600, 4000 Lumens, HDMI",                                          AssetState.Assigned,            new DateTime(2026, 3, 14)),
                Build("a0000000-0000-0000-0000-000000000093", CatProjector, HaNoiLocationId, "Projector ViewSonic PA503S","PJ000008", "SVGA 800x600, 3800 Lumens, HDMI",                                          AssetState.NotAvailable,        new DateTime(2026, 2, 19)),
                Build("a0000000-0000-0000-0000-000000000094", CatProjector, HaNoiLocationId, "Projector Acer X1228H",     "PJ000009", "XGA 1024x768, 4500 Lumens, HDMI",                                          AssetState.WaitingForRecycling, new DateTime(2026, 1, 11)),
                Build("a0000000-0000-0000-0000-000000000095", CatProjector, HaNoiLocationId, "Projector Optoma S336",     "PJ000010", "SVGA 800x600, 4000 Lumens, HDMI",                                          AssetState.Recycled,            new DateTime(2026, 2, 9)),

                Build("a0000000-0000-0000-0000-000000000096", CatNetworkSwitch, HaNoiLocationId, "Switch TP-Link SG1008D","NS000006", "8-Port Gigabit, Desktop, Fanless",                                           AssetState.Available,           new DateTime(2026, 4, 6)),
                Build("a0000000-0000-0000-0000-000000000097", CatNetworkSwitch, HaNoiLocationId, "Switch Cisco CBS250-8T","NS000007", "8-Port Gigabit, Managed, VLAN",                                               AssetState.Assigned,            new DateTime(2026, 3, 29)),
                Build("a0000000-0000-0000-0000-000000000098", CatNetworkSwitch, HaNoiLocationId, "Switch Netgear GS105",  "NS000008", "5-Port Gigabit, Desktop, Fanless",                                          AssetState.NotAvailable,        new DateTime(2026, 2, 15)),
                Build("a0000000-0000-0000-0000-000000000099", CatNetworkSwitch, HaNoiLocationId, "Switch D-Link DGS-1005A","NS000009","5-Port Gigabit, Green Ethernet",                                              AssetState.WaitingForRecycling, new DateTime(2026, 1, 19)),
                Build("a0000000-0000-0000-0000-000000000100", CatNetworkSwitch, HaNoiLocationId, "Switch MikroTik CSS106","NS000010", "5-Port Gigabit + 1 SFP, Managed",                                             AssetState.Recycled,            new DateTime(2026, 2, 24)),
            
                Build("a0000000-0000-0000-0000-000000001101", CatNetworkSwitch, HaNoiLocationId, "Switch TP-Link TL-SG108",      "NS000011", "8-Port Gigabit, Metal Case, Fanless",                     AssetState.Available, new DateTime(2026, 4, 12)),
                Build("a0000000-0000-0000-0000-000000001102", CatNetworkSwitch, HaNoiLocationId, "Switch Cisco SG110D-08",       "NS000012", "8-Port Gigabit, Unmanaged, Desktop",                      AssetState.Available, new DateTime(2026, 4, 14)),
                Build("a0000000-0000-0000-0000-000000001103", CatNetworkSwitch, HaNoiLocationId, "Switch Netgear GS308",         "NS000013", "8-Port Gigabit, Business Class",                          AssetState.Available, new DateTime(2026, 4, 16)),
                Build("a0000000-0000-0000-0000-000000001104", CatNetworkSwitch, HaNoiLocationId, "Switch D-Link DGS-108",        "NS000014", "8-Port Gigabit, Metal Housing",                           AssetState.Available, new DateTime(2026, 4, 18)),
                Build("a0000000-0000-0000-0000-000000001105", CatNetworkSwitch, HaNoiLocationId, "Switch MikroTik CRS112-8P",   "NS000015", "8-Port Gigabit, PoE, Managed",                            AssetState.Available, new DateTime(2026, 4, 20)),
                Build("a0000000-0000-0000-0000-000000001106", CatNetworkSwitch, HaNoiLocationId, "Switch Ubiquiti USW-Lite-8",  "NS000016", "8-Port Gigabit, Layer 2, Managed",                        AssetState.Available, new DateTime(2026, 4, 22)),
                Build("a0000000-0000-0000-0000-000000001107", CatNetworkSwitch, HaNoiLocationId, "Switch Aruba Instant On 1430", "NS000017", "8-Port Gigabit, Plug-and-Play",                           AssetState.Available, new DateTime(2026, 4, 24)),
                Build("a0000000-0000-0000-0000-000000001108", CatNetworkSwitch, HaNoiLocationId, "Switch Zyxel GS1200-8",       "NS000018", "8-Port Gigabit, Smart Managed",                           AssetState.Available, new DateTime(2026, 4, 26)),
                Build("a0000000-0000-0000-0000-000000001109", CatNetworkSwitch, HaNoiLocationId, "Switch Tenda SG108",           "NS000019", "8-Port Gigabit, Energy Saving",                           AssetState.Available, new DateTime(2026, 4, 28)),
                Build("a0000000-0000-0000-0000-000000001110", CatNetworkSwitch, HaNoiLocationId, "Switch Linksys LGS108",        "NS000020", "8-Port Gigabit, Business Desktop Switch",                 AssetState.Available, new DateTime(2026, 4, 30)),
            };
        }

        #endregion

        #region Ho Chi Minh Assets

        private static IEnumerable<Asset> GetHcmAssets()
        {
            return new List<Asset>
            {
                Build("a0000000-0000-0000-0000-000000000101", CatLaptop, HcmLocationId, "Laptop HP ProBook 450 G9", "LT000101", "Intel Core i5-1235U, 8GB RAM, 256GB SSD, 15.6\" FHD", AssetState.Available, new DateTime(2026, 1, 10)),
                Build("a0000000-0000-0000-0000-000000000102", CatLaptop, HcmLocationId, "Laptop Dell Latitude 5540", "LT000102", "Intel Core i7-1365U, 16GB RAM, 512GB SSD, 15.6\" FHD", AssetState.NotAvailable, new DateTime(2026, 1, 20)),
                Build("a0000000-0000-0000-0000-000000000103", CatLaptop, HcmLocationId, "Laptop Lenovo ThinkPad E15 Gen 4", "LT000103", "AMD Ryzen 5 5625U, 16GB RAM, 512GB SSD, 15.6\" FHD", AssetState.Assigned, new DateTime(2026, 2, 5)),
                Build("a0000000-0000-0000-0000-000000000104", CatLaptop, HcmLocationId, "Laptop Asus VivoBook 15 X1502", "LT000104", "Intel Core i5-12500H, 8GB RAM, 512GB SSD, 15.6\" FHD", AssetState.WaitingForRecycling, new DateTime(2026, 2, 15)),
                Build("a0000000-0000-0000-0000-000000000105", CatLaptop, HcmLocationId, "Laptop Acer Aspire 5 A515-57", "LT000105", "Intel Core i5-12450H, 8GB RAM, 512GB SSD, 15.6\" FHD", AssetState.Recycled, new DateTime(2026, 1, 5)),

                Build("a0000000-0000-0000-0000-000000000106", CatMonitor, HcmLocationId, "Monitor Dell UltraSharp U2422H", "MN000106", "24\" FHD IPS, 1920x1080, 60Hz, HDMI, DisplayPort, USB-C", AssetState.Available, new DateTime(2026, 1, 12)),
                Build("a0000000-0000-0000-0000-000000000107", CatMonitor, HcmLocationId, "Monitor LG 27UK850-W", "MN000107", "27\" 4K UHD IPS, 3840x2160, 60Hz, HDMI, DisplayPort, USB-C", AssetState.NotAvailable, new DateTime(2026, 1, 22)),
                Build("a0000000-0000-0000-0000-000000000108", CatMonitor, HcmLocationId, "Monitor Samsung F24T450FQE", "MN000108", "24\" FHD IPS, 1920x1080, 75Hz, HDMI, DisplayPort, Adjustable Stand", AssetState.Assigned, new DateTime(2026, 2, 8)),
                Build("a0000000-0000-0000-0000-000000000109", CatMonitor, HcmLocationId, "Monitor BenQ GW2480", "MN000109", "24\" FHD IPS, 1920x1080, 60Hz, HDMI, VGA, Eye-Care Technology", AssetState.WaitingForRecycling, new DateTime(2026, 3, 1)),
                Build("a0000000-0000-0000-0000-000000000110", CatMonitor, HcmLocationId, "Monitor AOC 22B2HM", "MN000110", "21.5\" FHD VA, 1920x1080, 75Hz, HDMI, VGA, Flicker-Free", AssetState.Recycled, new DateTime(2026, 1, 7)),

                Build("a0000000-0000-0000-0000-000000000111", CatKeyboard, HcmLocationId, "Keyboard Logitech MK270 Wireless", "KB000111", "Full-size, Wireless 2.4GHz, USB Receiver, Battery Life 24 months", AssetState.Available, new DateTime(2026, 1, 15)),
                Build("a0000000-0000-0000-0000-000000000112", CatKeyboard, HcmLocationId, "Keyboard Dell KB216 Wired", "KB000112", "Full-size, USB, Membrane Keys, Spill-resistant, 104 Keys", AssetState.NotAvailable, new DateTime(2026, 2, 1)),
                Build("a0000000-0000-0000-0000-000000000113", CatKeyboard, HcmLocationId, "Keyboard HP 125 Wired", "KB000113", "Full-size, USB, Quiet Keys, Adjustable Tilt Legs, 104 Keys", AssetState.Assigned, new DateTime(2026, 2, 20)),
                Build("a0000000-0000-0000-0000-000000000114", CatKeyboard, HcmLocationId, "Keyboard Rapoo E9050G Wireless", "KB000114", "Slim, Wireless 2.4GHz, Multi-mode, USB Receiver, Scissor Switch", AssetState.WaitingForRecycling, new DateTime(2026, 3, 5)),
                Build("a0000000-0000-0000-0000-000000000115", CatKeyboard, HcmLocationId, "Keyboard Genius KB-110X Wired", "KB000115", "Full-size, USB, Membrane, Spill-resistant, 104 Keys", AssetState.Recycled, new DateTime(2026, 1, 8)),

                Build("a0000000-0000-0000-0000-000000000116", CatMouse, HcmLocationId, "Mouse Logitech M100 Wired", "MS000116", "USB, Optical, 1000 DPI, Ambidextrous, Plug-and-Play", AssetState.Available, new DateTime(2026, 1, 18)),
                Build("a0000000-0000-0000-0000-000000000117", CatMouse, HcmLocationId, "Mouse Dell MS116 Wired", "MS000117", "USB, Optical, 1000 DPI, Right-handed, Scroll Wheel", AssetState.NotAvailable, new DateTime(2026, 2, 3)),
                Build("a0000000-0000-0000-0000-000000000118", CatMouse, HcmLocationId, "Mouse HP X500 Wired", "MS000118", "USB, Optical, 800/1200/1600 DPI, Ergonomic, Scroll Wheel", AssetState.Assigned, new DateTime(2026, 2, 25)),
                Build("a0000000-0000-0000-0000-000000000119", CatMouse, HcmLocationId, "Mouse Rapoo N1130 Wired", "MS000119", "USB, Optical, 1000 DPI, Ambidextrous, 3-Button Design", AssetState.WaitingForRecycling, new DateTime(2026, 3, 10)),
                Build("a0000000-0000-0000-0000-000000000120", CatMouse, HcmLocationId, "Mouse Genius DX-110 Wired", "MS000120", "USB, Optical, 1000 DPI, Scroll Wheel, Plug-and-Play", AssetState.Recycled, new DateTime(2026, 1, 6)),

                Build("a0000000-0000-0000-0000-000000000121", CatBluetoothMouse, HcmLocationId, "Bluetooth Mouse Logitech M650 L", "BM000121", "Bluetooth 5.0, 400-4000 DPI, Silent Click, 24-Month Battery Life", AssetState.Available, new DateTime(2026, 1, 25)),
                Build("a0000000-0000-0000-0000-000000000122", CatBluetoothMouse, HcmLocationId, "Bluetooth Mouse Microsoft Arc", "BM000122", "Bluetooth 5.0, BlueTrack, Foldable Design, 6-Month Battery Life", AssetState.NotAvailable, new DateTime(2026, 2, 10)),
                Build("a0000000-0000-0000-0000-000000000123", CatBluetoothMouse, HcmLocationId, "Bluetooth Mouse HP 240 Pike Silver", "BM000123", "Bluetooth 5.1, 1600 DPI, Silent Click, 16-Month Battery Life", AssetState.Assigned, new DateTime(2026, 3, 2)),
                Build("a0000000-0000-0000-0000-000000000124", CatBluetoothMouse, HcmLocationId, "Bluetooth Mouse Rapoo M650 Silent", "BM000124", "Bluetooth 3.0/5.0, 1600 DPI, Silent Buttons, 9-Month Battery Life", AssetState.WaitingForRecycling, new DateTime(2026, 3, 15)),
                Build("a0000000-0000-0000-0000-000000000125", CatBluetoothMouse, HcmLocationId, "Bluetooth Mouse Dell MS700", "BM000125", "Bluetooth 5.0, 1600 DPI, Ergonomic, 36-Month Battery Life", AssetState.Recycled, new DateTime(2026, 1, 9)),

                Build("a0000000-0000-0000-0000-000000000126", CatBatteryMonitor, HcmLocationId, "Battery Monitor Eaton 5E 1100i USB", "BM1000126", "1100VA/660W, USB, 4 Output Sockets, Protection Time 9 min at full load", AssetState.Available, new DateTime(2026, 1, 28)),
                Build("a0000000-0000-0000-0000-000000000127", CatBatteryMonitor, HcmLocationId, "Battery Monitor APC Back-UPS BX1000M", "BM1000127", "1000VA/600W, USB, 8 Output Sockets, LCD Display, AVR", AssetState.NotAvailable, new DateTime(2026, 2, 12)),
                Build("a0000000-0000-0000-0000-000000000128", CatBatteryMonitor, HcmLocationId, "Battery Monitor CyberPower CP900EPFCLCD", "BM1000128", "900VA/540W, USB, 8 Output Sockets, LCD Panel, Pure Sine Wave", AssetState.Assigned, new DateTime(2026, 3, 5)),
                Build("a0000000-0000-0000-0000-000000000129", CatBatteryMonitor, HcmLocationId, "Battery Monitor Santak TG-BOX 850", "BM1000129", "850VA/510W, USB, 4 Output Sockets, AVR, Backup Time 9 min", AssetState.WaitingForRecycling, new DateTime(2026, 3, 18)),
                Build("a0000000-0000-0000-0000-000000000130", CatBatteryMonitor, HcmLocationId, "Battery Monitor Ares AR-800VA", "BM1000130", "800VA/480W, USB, 4 Output Sockets, AVR, Backup Time 7 min", AssetState.Recycled, new DateTime(2026, 1, 11)),

                Build("a0000000-0000-0000-0000-000000000131", CatPrinter, HcmLocationId, "Printer HP LaserJet Pro M404dn", "PR000131", "A4, Mono Laser, 38ppm, Duplex, USB, LAN, 250-Sheet Tray", AssetState.Available, new DateTime(2026, 1, 30)),
                Build("a0000000-0000-0000-0000-000000000132", CatPrinter, HcmLocationId, "Printer Canon imageCLASS LBP6030", "PR000132", "A4, Mono Laser, 18ppm, USB, 150-Sheet Tray, Compact Design", AssetState.NotAvailable, new DateTime(2026, 2, 14)),
                Build("a0000000-0000-0000-0000-000000000133", CatPrinter, HcmLocationId, "Printer Epson EcoTank L3210", "PR000133", "A4, Color Inkjet, 10ppm Black/5ppm Color, USB, Ink Tank System", AssetState.Assigned, new DateTime(2026, 3, 8)),
                Build("a0000000-0000-0000-0000-000000000134", CatPrinter, HcmLocationId, "Printer Brother DCP-L2540DW", "PR000134", "A4, Mono Laser MFP, 30ppm, Duplex, WiFi, USB, LAN, 250-Sheet Tray", AssetState.WaitingForRecycling, new DateTime(2026, 3, 20)),
                Build("a0000000-0000-0000-0000-000000000135", CatPrinter, HcmLocationId, "Printer Samsung SL-M2020W", "PR000135", "A4, Mono Laser, 21ppm, WiFi, USB, 150-Sheet Tray", AssetState.Recycled, new DateTime(2026, 1, 13)),

                Build("a0000000-0000-0000-0000-000000000136", CatScanner, HcmLocationId, "Scanner Canon DR-C225W II", "SC000136", "A4, 25ppm/50ipm, ADF 30-Sheet, WiFi, USB, Duplex, CIS Sensor", AssetState.Available, new DateTime(2026, 2, 2)),
                Build("a0000000-0000-0000-0000-000000000137", CatScanner, HcmLocationId, "Scanner Epson DS-530 II", "SC000137", "A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI", AssetState.NotAvailable, new DateTime(2026, 2, 18)),
                Build("a0000000-0000-0000-0000-000000000138", CatScanner, HcmLocationId, "Scanner Fujitsu fi-7030", "SC000138", "A4, 27ppm/54ipm, ADF 50-Sheet, USB, Duplex, CCD Sensor, 600 DPI", AssetState.Assigned, new DateTime(2026, 3, 12)),
                Build("a0000000-0000-0000-0000-000000000139", CatScanner, HcmLocationId, "Scanner HP ScanJet Pro 2000 s2", "SC000139", "A4, 35ppm/70ipm, ADF 50-Sheet, USB, Duplex, CIS Sensor, 600 DPI", AssetState.WaitingForRecycling, new DateTime(2026, 3, 22)),
                Build("a0000000-0000-0000-0000-000000000140", CatScanner, HcmLocationId, "Scanner Brother ADS-1200", "SC000140", "A4, 25ppm/50ipm, ADF 20-Sheet, USB, Duplex, CIS Sensor, 600 DPI", AssetState.Recycled, new DateTime(2026, 1, 16)),

                Build("a0000000-0000-0000-0000-000000000141", CatProjector, HcmLocationId, "Projector Epson EB-X49", "PJ000141", "XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, Lamp 12000h", AssetState.Available, new DateTime(2026, 2, 5)),
                Build("a0000000-0000-0000-0000-000000000142", CatProjector, HcmLocationId, "Projector Benq MX550", "PJ000142", "XGA 1024x768, 3600 Lumens, HDMI, USB, VGA, SmartEco Mode", AssetState.NotAvailable, new DateTime(2026, 2, 22)),
                Build("a0000000-0000-0000-0000-000000000143", CatProjector, HcmLocationId, "Projector ViewSonic PA503X", "PJ000143", "XGA 1024x768, 3800 Lumens, HDMI, USB, VGA, Lamp 15000h", AssetState.Assigned, new DateTime(2026, 3, 15)),
                Build("a0000000-0000-0000-0000-000000000144", CatProjector, HcmLocationId, "Projector Optoma X400LVe", "PJ000144", "XGA 1024x768, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h ECO", AssetState.WaitingForRecycling, new DateTime(2026, 4, 1)),
                Build("a0000000-0000-0000-0000-000000000145", CatProjector, HcmLocationId, "Projector Acer X1526HK", "PJ000145", "FHD 1920x1080, 4000 Lumens, HDMI, USB, VGA, Lamp 15000h", AssetState.Recycled, new DateTime(2026, 1, 19)),

                Build("a0000000-0000-0000-0000-000000000146", CatTablet, HcmLocationId, "Tablet Samsung Galaxy Tab A8", "TB000146", "10.5\" TFT, Unisoc T618, 4GB RAM, 64GB, WiFi, Android 11", AssetState.Available, new DateTime(2026, 2, 8)),
                Build("a0000000-0000-0000-0000-000000000147", CatTablet, HcmLocationId, "Tablet Lenovo Tab M10 Plus Gen 3", "TB000147", "10.61\" 2K IPS, Snapdragon 680, 4GB RAM, 128GB, WiFi, Android 12", AssetState.NotAvailable, new DateTime(2026, 2, 26)),
                Build("a0000000-0000-0000-0000-000000000148", CatTablet, HcmLocationId, "Tablet Huawei MatePad 11.5", "TB000148", "11.5\" IPS 120Hz, Snapdragon 7 Gen 1, 8GB RAM, 128GB, WiFi", AssetState.Assigned, new DateTime(2026, 3, 18)),
                Build("a0000000-0000-0000-0000-000000000149", CatTablet, HcmLocationId, "Tablet Xiaomi Pad 6", "TB000149", "11\" IPS 144Hz, Snapdragon 870, 8GB RAM, 256GB, WiFi, MIUI 14", AssetState.WaitingForRecycling, new DateTime(2026, 4, 5)),
                Build("a0000000-0000-0000-0000-000000000150", CatTablet, HcmLocationId, "Tablet Amazon Fire HD 10", "TB000150", "10.1\" FHD IPS, Octa-Core 2.0GHz, 3GB RAM, 32GB, WiFi, FireOS", AssetState.Recycled, new DateTime(2026, 1, 23)),

                Build("a0000000-0000-0000-0000-000000000151", CatDesktop, HcmLocationId, "Desktop Dell OptiPlex 3000 MT", "DC000151", "Intel Core i5-12500, 8GB RAM, 256GB SSD, Intel UHD 770, Win 11 Pro", AssetState.Available, new DateTime(2026, 2, 10)),
                Build("a0000000-0000-0000-0000-000000000152", CatDesktop, HcmLocationId, "Desktop HP EliteDesk 800 G9 SFF", "DC000152", "Intel Core i7-12700, 16GB RAM, 512GB SSD, Intel UHD 770, Win 11 Pro", AssetState.NotAvailable, new DateTime(2026, 3, 1)),
                Build("a0000000-0000-0000-0000-000000000153", CatDesktop, HcmLocationId, "Desktop Lenovo ThinkCentre M70s Gen 3", "DC000153", "Intel Core i5-12400, 8GB RAM, 256GB SSD, Intel UHD 730, Win 11 Pro", AssetState.Assigned, new DateTime(2026, 3, 20)),
                Build("a0000000-0000-0000-0000-000000000154", CatDesktop, HcmLocationId, "Desktop Asus ExpertCenter D5 SFF D500SC", "DC000154", "Intel Core i3-10105, 8GB RAM, 256GB SSD, Intel UHD 630, Win 11 Pro", AssetState.WaitingForRecycling, new DateTime(2026, 4, 8)),
                Build("a0000000-0000-0000-0000-000000000155", CatDesktop, HcmLocationId, "Desktop Acer Veriton M6680G", "DC000155", "Intel Core i5-10400, 8GB RAM, 256GB SSD, Intel UHD 630, Win 10 Pro", AssetState.Recycled, new DateTime(2026, 1, 26)),

                Build("a0000000-0000-0000-0000-000000000156", CatNetworkSwitch, HcmLocationId, "Network Switch Cisco CBS110-16T", "NS000156", "16-Port Gigabit, Unmanaged, Desktop, Fanless, QoS, IEEE 802.3az", AssetState.Available, new DateTime(2026, 2, 15)),
                Build("a0000000-0000-0000-0000-000000000157", CatNetworkSwitch, HcmLocationId, "Network Switch TP-Link TL-SG1024D", "NS000157", "24-Port Gigabit, Unmanaged, Desktop/Rack-mount, Fanless, IEEE 802.3az", AssetState.NotAvailable, new DateTime(2026, 3, 3)),
                Build("a0000000-0000-0000-0000-000000000158", CatNetworkSwitch, HcmLocationId, "Network Switch D-Link DGS-1016D", "NS000158", "16-Port Gigabit, Unmanaged, Desktop, Fanless, Auto MDI/MDIX", AssetState.Assigned, new DateTime(2026, 3, 25)),
                Build("a0000000-0000-0000-0000-000000000159", CatNetworkSwitch, HcmLocationId, "Network Switch Netgear GS308E", "NS000159", "8-Port Gigabit, Smart Managed, Desktop, QoS, VLAN, IGMP Snooping", AssetState.WaitingForRecycling, new DateTime(2026, 4, 10)),
                Build("a0000000-0000-0000-0000-000000000160", CatNetworkSwitch, HcmLocationId, "Network Switch MikroTik CRS112-8G-4S-IN", "NS000160", "8-Port Gigabit + 4 SFP, Managed, Desktop, RouterOS/SwitchOS, PoE", AssetState.Recycled, new DateTime(2026, 1, 29)),

                Build("a0000000-0000-0000-0000-000000000161", CatLaptop, HcmLocationId, "Laptop MSI Modern 15", "LT000161", "Intel Core i7-1255U, 16GB RAM, 512GB SSD, 15.6\" FHD", AssetState.Available, new DateTime(2026, 4, 12)),
                Build("a0000000-0000-0000-0000-000000000162", CatLaptop, HcmLocationId, "Laptop Dell Inspiron 15", "LT000162", "Intel Core i5-1335U, 8GB RAM, 512GB SSD, 15.6\" FHD", AssetState.NotAvailable, new DateTime(2026, 4, 14)),
                Build("a0000000-0000-0000-0000-000000000163", CatLaptop, HcmLocationId, "Laptop HP Pavilion 14", "LT000163", "Intel Core i5-1240P, 16GB RAM, 512GB SSD, 14\" FHD", AssetState.Assigned, new DateTime(2026, 4, 16)),
                Build("a0000000-0000-0000-0000-000000000164", CatLaptop, HcmLocationId, "Laptop Lenovo IdeaPad Slim 5", "LT000164", "AMD Ryzen 7 7730U, 16GB RAM, 512GB SSD, 14\" OLED", AssetState.WaitingForRecycling, new DateTime(2026, 4, 18)),
                Build("a0000000-0000-0000-0000-000000000165", CatLaptop, HcmLocationId, "Laptop Asus ZenBook 14", "LT000165", "Intel Core Ultra 5, 16GB RAM, 1TB SSD, 14\" OLED", AssetState.Recycled, new DateTime(2026, 4, 20)),

                Build("a0000000-0000-0000-0000-000000000166", CatMonitor, HcmLocationId, "Monitor LG 24MP400", "MN000166", "24\" FHD IPS, 75Hz, HDMI, VGA", AssetState.Available, new DateTime(2026, 4, 1)),
                Build("a0000000-0000-0000-0000-000000000167", CatMonitor, HcmLocationId, "Monitor Samsung Odyssey G3", "MN000167", "24\" FHD VA, 144Hz, HDMI, DisplayPort", AssetState.NotAvailable, new DateTime(2026, 4, 3)),
                Build("a0000000-0000-0000-0000-000000000168", CatMonitor, HcmLocationId, "Monitor Asus VA24EHF", "MN000168", "23.8\" FHD IPS, 100Hz, HDMI", AssetState.Assigned, new DateTime(2026, 4, 5)),
                Build("a0000000-0000-0000-0000-000000000169", CatMonitor, HcmLocationId, "Monitor ViewSonic VA2432-H", "MN000169", "24\" FHD IPS, 75Hz, HDMI, VGA", AssetState.WaitingForRecycling, new DateTime(2026, 4, 7)),
                Build("a0000000-0000-0000-0000-000000000170", CatMonitor, HcmLocationId, "Monitor Acer EK241Y", "MN000170", "23.8\" FHD IPS, 100Hz, HDMI, VGA", AssetState.Recycled, new DateTime(2026, 4, 9)),

                Build("a0000000-0000-0000-0000-000000000171", CatKeyboard, HcmLocationId, "Keyboard Logitech K120", "KB000171", "USB, Full-size, Spill Resistant", AssetState.Available, new DateTime(2026, 4, 11)),
                Build("a0000000-0000-0000-0000-000000000172", CatKeyboard, HcmLocationId, "Keyboard Rapoo NK2600", "KB000172", "USB, Full-size, Membrane", AssetState.NotAvailable, new DateTime(2026, 4, 13)),
                Build("a0000000-0000-0000-0000-000000000173", CatKeyboard, HcmLocationId, "Keyboard DareU LK145", "KB000173", "USB, Mechanical Feel, LED", AssetState.Assigned, new DateTime(2026, 4, 15)),
                Build("a0000000-0000-0000-0000-000000000174", CatKeyboard, HcmLocationId, "Keyboard Fuhlen L411", "KB000174", "USB, Full-size, Quiet Keys", AssetState.WaitingForRecycling, new DateTime(2026, 4, 17)),
                Build("a0000000-0000-0000-0000-000000000175", CatKeyboard, HcmLocationId, "Keyboard HP GK100", "KB000175", "USB, RGB Backlit, Mechanical", AssetState.Recycled, new DateTime(2026, 4, 19)),

                Build("a0000000-0000-0000-0000-000000000176", CatMouse, HcmLocationId, "Mouse Logitech B100", "MS000176", "USB Optical Mouse 800 DPI", AssetState.Available, new DateTime(2026, 4, 2)),
                Build("a0000000-0000-0000-0000-000000000177", CatMouse, HcmLocationId, "Mouse Rapoo N200", "MS000177", "USB Optical Mouse 1000 DPI", AssetState.NotAvailable, new DateTime(2026, 4, 4)),
                Build("a0000000-0000-0000-0000-000000000178", CatMouse, HcmLocationId, "Mouse Dell MS3320W", "MS000178", "Wireless Mouse 1600 DPI", AssetState.Assigned, new DateTime(2026, 4, 6)),
                Build("a0000000-0000-0000-0000-000000000179", CatMouse, HcmLocationId, "Mouse HP Z3700", "MS000179", "Wireless Optical Mouse", AssetState.WaitingForRecycling, new DateTime(2026, 4, 8)),
                Build("a0000000-0000-0000-0000-000000000180", CatMouse, HcmLocationId, "Mouse Asus MD100", "MS000180", "Wireless Silent Mouse", AssetState.Recycled, new DateTime(2026, 4, 10)),

                Build("a0000000-0000-0000-0000-000000000181", CatPrinter, HcmLocationId, "Printer Brother HL-L2321D", "PR000181", "Mono Laser, Duplex Printing", AssetState.Available, new DateTime(2026, 4, 12)),
                Build("a0000000-0000-0000-0000-000000000182", CatPrinter, HcmLocationId, "Printer Canon G3010", "PR000182", "Color Ink Tank, WiFi", AssetState.NotAvailable, new DateTime(2026, 4, 14)),
                Build("a0000000-0000-0000-0000-000000000183", CatPrinter, HcmLocationId, "Printer Epson L3250", "PR000183", "Color Ink Tank, WiFi Direct", AssetState.Assigned, new DateTime(2026, 4, 16)),
                Build("a0000000-0000-0000-0000-000000000184", CatPrinter, HcmLocationId, "Printer HP Smart Tank 520", "PR000184", "All-in-One Ink Tank Printer", AssetState.WaitingForRecycling, new DateTime(2026, 4, 18)),
                Build("a0000000-0000-0000-0000-000000000185", CatPrinter, HcmLocationId, "Printer Pantum P2500W", "PR000185", "Mono Laser Wireless Printer", AssetState.Recycled, new DateTime(2026, 4, 20)),

                Build("a0000000-0000-0000-0000-000000000186", CatScanner, HcmLocationId, "Scanner Canon CanoScan LiDE 300", "SC000186", "Flatbed Scanner, 2400 DPI", AssetState.Available, new DateTime(2026, 4, 1)),
                Build("a0000000-0000-0000-0000-000000000187", CatScanner, HcmLocationId, "Scanner Epson Perfection V39", "SC000187", "Flatbed Scanner, USB Powered", AssetState.NotAvailable, new DateTime(2026, 4, 3)),
                Build("a0000000-0000-0000-0000-000000000188", CatScanner, HcmLocationId, "Scanner Brother ADS-1700W", "SC000188", "Wireless Document Scanner", AssetState.Assigned, new DateTime(2026, 4, 5)),
                Build("a0000000-0000-0000-0000-000000000189", CatScanner, HcmLocationId, "Scanner Fujitsu ScanSnap iX1600", "SC000189", "Duplex WiFi Scanner", AssetState.WaitingForRecycling, new DateTime(2026, 4, 7)),
                Build("a0000000-0000-0000-0000-000000000190", CatScanner, HcmLocationId, "Scanner HP ScanJet Pro 2600", "SC000190", "ADF Duplex Scanner", AssetState.Recycled, new DateTime(2026, 4, 9)),

                Build("a0000000-0000-0000-0000-000000000191", CatProjector, HcmLocationId, "Projector Epson CO-FH02", "PJ000191", "Full HD, 3000 Lumens", AssetState.Available, new DateTime(2026, 4, 11)),
                Build("a0000000-0000-0000-0000-000000000192", CatProjector, HcmLocationId, "Projector BenQ TH575", "PJ000192", "Full HD Gaming Projector", AssetState.NotAvailable, new DateTime(2026, 4, 13)),
                Build("a0000000-0000-0000-0000-000000000193", CatProjector, HcmLocationId, "Projector ViewSonic PX701HDH", "PJ000193", "3500 Lumens Full HD", AssetState.Assigned, new DateTime(2026, 4, 15)),
                Build("a0000000-0000-0000-0000-000000000194", CatProjector, HcmLocationId, "Projector Acer X1228H", "PJ000194", "XGA Office Projector", AssetState.WaitingForRecycling, new DateTime(2026, 4, 17)),
                Build("a0000000-0000-0000-0000-000000000195", CatProjector, HcmLocationId, "Projector Optoma HD146X", "PJ000195", "1080p Home Projector", AssetState.Recycled, new DateTime(2026, 4, 19)),

                Build("a0000000-0000-0000-0000-000000000196", CatTablet, HcmLocationId, "Tablet iPad Gen 10", "TB000196", "10.9\" Retina, A14 Bionic, 64GB WiFi", AssetState.Available, new DateTime(2026, 4, 2)),
                Build("a0000000-0000-0000-0000-000000000197", CatTablet, HcmLocationId, "Tablet Samsung Galaxy Tab S9 FE", "TB000197", "10.9\" WQXGA, Exynos 1380, 128GB", AssetState.NotAvailable, new DateTime(2026, 4, 4)),
                Build("a0000000-0000-0000-0000-000000000198", CatTablet, HcmLocationId, "Tablet Xiaomi Redmi Pad SE", "TB000198", "11\" FHD+, Snapdragon 680", AssetState.Assigned, new DateTime(2026, 4, 6)),
                Build("a0000000-0000-0000-0000-000000000199", CatTablet, HcmLocationId, "Tablet Lenovo Xiaoxin Pad", "TB000199", "11.5\" 2K, Snapdragon 685", AssetState.WaitingForRecycling, new DateTime(2026, 4, 8)),
                Build("a0000000-0000-0000-0000-000000000200", CatTablet, HcmLocationId, "Tablet Huawei MatePad Air", "TB000200", "11.5\" 2.8K, Snapdragon 888", AssetState.Recycled, new DateTime(2026, 4, 10)),

            };
        }

        #endregion

        private static Asset Build(
            string id,
            Guid categoryId,
            Guid locationId,
            string name,
            string code,
            string specification,
            AssetState state,
            DateTime installedDate)
        {
            var utcDate = DateTime.SpecifyKind(installedDate, DateTimeKind.Utc);
            return new AssetBuilder()
                .WithId(Guid.Parse(id))
                .WithCategoryId(categoryId)
                .WithLocationId(locationId)
                .WithAssetName(name)
                .WithAssetCode(code)
                .WithAssetSpecification(specification)
                .WithAssetState(state)
                .WithInstalledDateAtUtc(utcDate)
                .WithCreatedAtUtc(utcDate)
                .WithUpdatedAtUtc(null)
                .Build();
        }
    }
}
