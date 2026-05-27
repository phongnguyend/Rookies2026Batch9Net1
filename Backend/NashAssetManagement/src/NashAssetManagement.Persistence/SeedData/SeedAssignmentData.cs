using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.Persistence.Builder;

namespace NashAssetManagement.Persistence.SeedData
{
    public static class SeedAssignmentData
    {
        public static List<Assignment> GetData()
        {
            var assignments = new List<Assignment>();
            assignments.AddRange(GetHanoiAssignments());
            assignments.AddRange(GetHcmAssignments());
            return assignments;
        }

        // ── Ha Noi ──────────────────────────────────────────────────────────────
        private static IEnumerable<Assignment> GetHanoiAssignments()
        {
            return new List<Assignment>
                    {
                Build("b0000000-0000-0000-0000-000000001101",
                    assetId:        "a0000000-0000-0000-0000-000000001101",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 25),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001102",
                    assetId:        "a0000000-0000-0000-0000-000000001102",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 26),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001103",
                    assetId:        "a0000000-0000-0000-0000-000000001103",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 24),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001104",
                    assetId:        "a0000000-0000-0000-0000-000000001104",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 25),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001105",
                    assetId:        "a0000000-0000-0000-0000-000000001105",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 22),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001106",
                    assetId:        "a0000000-0000-0000-0000-000000001106",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 20),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001107",
                    assetId:        "a0000000-0000-0000-0000-000000001107",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 24),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001108",
                    assetId:        "a0000000-0000-0000-0000-000000001108",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 15),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001109",
                    assetId:        "a0000000-0000-0000-0000-000000001109",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 18),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000001110",
                    assetId:        "a0000000-0000-0000-0000-000000001110",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 25),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                // Asset: LT000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000001",
                    assetId:        "a0000000-0000-0000-0000-000000000001",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 5, 13),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: MN000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000002",
                    assetId:        "a0000000-0000-0000-0000-000000000006",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 10),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: KB000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000003",
                    assetId:        "a0000000-0000-0000-0000-000000000011",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 27),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: MS000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000004",
                    assetId:        "a0000000-0000-0000-0000-000000000016",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 5, 23),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: BM000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000005",
                    assetId:        "a0000000-0000-0000-0000-000000000021",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 30),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: BM1000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000006",
                    assetId:        "a0000000-0000-0000-0000-000000000026",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 31),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: PR000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000007",
                    assetId:        "a0000000-0000-0000-0000-000000000031",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 5, 27),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: SC000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000008",
                    assetId:        "a0000000-0000-0000-0000-000000000036",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 15),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: PJ000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000009",
                    assetId:        "a0000000-0000-0000-0000-000000000041",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 24),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: TB000001 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000010",
                    assetId:        "a0000000-0000-0000-0000-000000000046",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 5, 28),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: DC000001 | Returned
                // FIX: isReturning = false vì asset đã hoàn trả xong (state = Returned)
                Build("b0000000-0000-0000-0000-000000000011",
                    assetId:        "a0000000-0000-0000-0000-000000000051",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 7),
                    state:          AssignmentState.Returned,
                    isReturning:    false),
 
                // Asset: NS000001 | Returned
                // FIX: isReturning = false vì asset đã hoàn trả xong (state = Returned)
                Build("b0000000-0000-0000-0000-000000000012",
                    assetId:        "a0000000-0000-0000-0000-000000000056",
                    assignedTo:     "10000000-0000-0000-0000-000000000004",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 12),
                    state:          AssignmentState.Returned,
                    isReturning:    false),
 
                // Asset: LT000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000013",
                    assetId:        "a0000000-0000-0000-0000-000000000003",
                    assignedTo:     "10000000-0000-0000-0000-000000000005",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 4, 27),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: MN000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000014",
                    assetId:        "a0000000-0000-0000-0000-000000000008",
                    assignedTo:     "10000000-0000-0000-0000-000000000006",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 9),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: KB000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000015",
                    assetId:        "a0000000-0000-0000-0000-000000000013",
                    assignedTo:     "10000000-0000-0000-0000-000000000007",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 15),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: MS000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000016",
                    assetId:        "a0000000-0000-0000-0000-000000000018",
                    assignedTo:     "10000000-0000-0000-0000-000000000008",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 4, 17),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: BM000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000017",
                    assetId:        "a0000000-0000-0000-0000-000000000023",
                    assignedTo:     "10000000-0000-0000-0000-000000000009",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 6),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: BM1000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000018",
                    assetId:        "a0000000-0000-0000-0000-000000000028",
                    assignedTo:     "10000000-0000-0000-0000-000000000010",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 10),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: PR000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000019",
                    assetId:        "a0000000-0000-0000-0000-000000000033",
                    assignedTo:     "10000000-0000-0000-0000-000000000011",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 5, 18),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: SC000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000020",
                    assetId:        "a0000000-0000-0000-0000-000000000038",
                    assignedTo:     "10000000-0000-0000-0000-000000000012",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 5, 13),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: PJ000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000021",
                    assetId:        "a0000000-0000-0000-0000-000000000043",
                    assignedTo:     "10000000-0000-0000-0000-000000000013",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 16),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: TB000003 | Accepted
                Build("b0000000-0000-0000-0000-000000000022",
                    assetId:        "a0000000-0000-0000-0000-000000000048",
                    assignedTo:     "10000000-0000-0000-0000-000000000014",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 5, 11),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: DC000003 | Accepted (IsReturning = true)
                Build("b0000000-0000-0000-0000-000000000023",
                    assetId:        "a0000000-0000-0000-0000-000000000053",
                    assignedTo:     "10000000-0000-0000-0000-000000000015",
                    assignedBy:     "10000000-0000-0000-0000-000000000002",
                    assignedDate:   new DateTime(2026, 4, 22),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),
 
                // Asset: NS000003 | Accepted (IsReturning = true)
                Build("b0000000-0000-0000-0000-000000000024",
                    assetId:        "a0000000-0000-0000-0000-000000000058",
                    assignedTo:     "10000000-0000-0000-0000-000000000005",
                    assignedBy:     "10000000-0000-0000-0000-000000000003",
                    assignedDate:   new DateTime(2026, 5, 5),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),
 
                // Asset: LT000007 | Accepted (IsReturning = true)
                Build("b0000000-0000-0000-0000-000000000025",
                    assetId:        "a0000000-0000-0000-0000-000000000062",
                    assignedTo:     "10000000-0000-0000-0000-000000000006",
                    assignedBy:     "10000000-0000-0000-0000-000000000001",
                    assignedDate:   new DateTime(2026, 5, 14),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),
            };
        }

        // ── Ho Chi Minh ─────────────────────────────────────────────────────────
        // FIX: Đổi toàn bộ ID sang dải b0000000-...-0000000000xx bắt đầu từ 026
        //      để tránh trùng với dải HN (001–025)
        private static IEnumerable<Assignment> GetHcmAssignments()
        {
            return new List<Assignment>
            {
                // ── Special Staff: lanlt gets many assignments ─────────────────
 
                // Asset: LT000006 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000026",
                    assetId:        "a0000000-0000-0000-0000-000000000061",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000016", // annh
                    assignedDate:   new DateTime(2026, 5, 23),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: MN000006 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000027",
                    assetId:        "a0000000-0000-0000-0000-000000000066",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000017", // trangpn
                    assignedDate:   new DateTime(2026, 5, 27),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: KB000006 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000028",
                    assetId:        "a0000000-0000-0000-0000-000000000071",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000018", // kiettm
                    assignedDate:   new DateTime(2026, 6, 2),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: MS000006 | WaitingForAcceptance
                Build("b0000000-0000-0000-0000-000000000029",
                    assetId:        "a0000000-0000-0000-0000-000000000076",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000016", // annh
                    assignedDate:   new DateTime(2026, 6, 9),
                    state:          AssignmentState.WaitingForAcceptance,
                    isReturning:    false),
 
                // Asset: BM000006 | Returned
                // FIX: isReturning = false vì asset đã hoàn trả xong (state = Returned)
                Build("b0000000-0000-0000-0000-000000000030",
                    assetId:        "a0000000-0000-0000-0000-000000000081",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000016", // annh
                    assignedDate:   new DateTime(2026, 1, 18),
                    state:          AssignmentState.Returned,
                    isReturning:    false),
 
                // Asset: BM1000006 | Returned
                // FIX: isReturning = false vì asset đã hoàn trả xong (state = Returned)
                Build("b0000000-0000-0000-0000-000000000031",
                    assetId:        "a0000000-0000-0000-0000-000000000086",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000017", // trangpn
                    assignedDate:   new DateTime(2026, 2, 3),
                    state:          AssignmentState.Returned,
                    isReturning:    false),
 
                // Asset: PR000006 | Returned
                // FIX: isReturning = false vì asset đã hoàn trả xong (state = Returned)
                Build("b0000000-0000-0000-0000-000000000032",
                    assetId:        "a0000000-0000-0000-0000-000000000091",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000018", // kiettm
                    assignedDate:   new DateTime(2026, 2, 22),
                    state:          AssignmentState.Returned,
                    isReturning:    false),
 
                // Asset: SC000006 | Returned
                // FIX: isReturning = false vì asset đã hoàn trả xong (state = Returned)
                Build("b0000000-0000-0000-0000-000000000033",
                    assetId:        "a0000000-0000-0000-0000-000000000096",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000016", // annh
                    assignedDate:   new DateTime(2026, 3, 12),
                    state:          AssignmentState.Returned,
                    isReturning:    false),
 
                // Asset: LT000008 | Accepted (IsReturning = true)
                Build("b0000000-0000-0000-0000-000000000034",
                    assetId:        "a0000000-0000-0000-0000-000000000063",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000016", // annh
                    assignedDate:   new DateTime(2026, 2, 7),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),
 
                // Asset: MN000008 | Accepted
                Build("b0000000-0000-0000-0000-000000000035",
                    assetId:        "a0000000-0000-0000-0000-000000000068",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000017", // trangpn
                    assignedDate:   new DateTime(2026, 2, 27),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // Asset: KB000008 | Accepted (IsReturning = true)
                Build("b0000000-0000-0000-0000-000000000036",
                    assetId:        "a0000000-0000-0000-0000-000000000073",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000018", // kiettm
                    assignedDate:   new DateTime(2026, 3, 18),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),
 
                // Asset: MS000008 | Accepted
                Build("b0000000-0000-0000-0000-000000000037",
                    assetId:        "a0000000-0000-0000-0000-000000000078",
                    assignedTo:     "10000000-0000-0000-0000-000000000019", // lanlt
                    assignedBy:     "10000000-0000-0000-0000-000000000016", // annh
                    assignedDate:   new DateTime(2026, 4, 8),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),
 
                // ── Other HCM Staff ────────────────────────────────────────────
 
                Build("b0000000-0000-0000-0000-000000000038",
                    assetId:        "a0000000-0000-0000-0000-000000000062",
                    assignedTo:     "10000000-0000-0000-0000-000000000020", // hieuvt
                    assignedBy:     "10000000-0000-0000-0000-000000000016",
                    assignedDate:   new DateTime(2026, 3, 5),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000000039",
                    assetId:        "a0000000-0000-0000-0000-000000000067",
                    assignedTo:     "10000000-0000-0000-0000-000000000021", // mydb
                    assignedBy:     "10000000-0000-0000-0000-000000000017",
                    assignedDate:   new DateTime(2026, 3, 10),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),

                Build("b0000000-0000-0000-0000-000000000040",
                    assetId:        "a0000000-0000-0000-0000-000000000072",
                    assignedTo:     "10000000-0000-0000-0000-000000000022", // ducbg
                    assignedBy:     "10000000-0000-0000-0000-000000000018",
                    assignedDate:   new DateTime(2026, 3, 15),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000000041",
                    assetId:        "a0000000-0000-0000-0000-000000000077",
                    assignedTo:     "10000000-0000-0000-0000-000000000023", // nhiht
                    assignedBy:     "10000000-0000-0000-0000-000000000016",
                    assignedDate:   new DateTime(2026, 3, 20),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),

                Build("b0000000-0000-0000-0000-000000000042",
                    assetId:        "a0000000-0000-0000-0000-000000000082",
                    assignedTo:     "10000000-0000-0000-0000-000000000024", // taipa
                    assignedBy:     "10000000-0000-0000-0000-000000000017",
                    assignedDate:   new DateTime(2026, 4, 1),
                    state:          AssignmentState.Accepted,
                    isReturning:    false),

                Build("b0000000-0000-0000-0000-000000000043",
                    assetId:        "a0000000-0000-0000-0000-000000000087",
                    assignedTo:     "10000000-0000-0000-0000-000000000025", // chaunm
                    assignedBy:     "10000000-0000-0000-0000-000000000018",
                    assignedDate:   new DateTime(2026, 4, 5),
                    state:          AssignmentState.Accepted,
                    isReturning:    true),
            };
        }

        private static Assignment Build(
            string id,
            string assetId,
            string assignedTo,
            string assignedBy,
            DateTime assignedDate,
            AssignmentState state,
            bool isReturning)
        {
            var utcDate = DateTime.SpecifyKind(assignedDate, DateTimeKind.Utc);
            return new AssignmentBuilder()
                .WithId(Guid.Parse(id))
                .WithAssetId(Guid.Parse(assetId))
                .WithAssignedToUserId(Guid.Parse(assignedTo))
                .WithAssignedByUserId(Guid.Parse(assignedBy))
                .WithAssignedDateAtUtc(utcDate)
                .WithState(state)
                .WithIsReturning(isReturning)
                .WithCreatedAtUtc(utcDate)
                .WithUpdatedAtUtc(null)
                .Build();
        }
    }
}

