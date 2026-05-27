using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;
using NashAssetManagement.Persistence.Builder;

namespace NashAssetManagement.Persistence.SeedData
{
    public static class SeedReturnRequestData
    {
        public static List<ReturnRequest> GetData() => new List<ReturnRequest>
    {
        // ════════════════════════════════════════════════════════════════════════
        // HA NOI – WaitingForReturning  (Assignment: Accepted + isReturning=true)
        // ════════════════════════════════════════════════════════════════════════
 
        // Assignment b023 | Asset DC000003 | assignedTo: user015 | assignedBy: admin002
        Build("c0000000-0000-0000-0000-000000000001",
            assignmentId:   "b0000000-0000-0000-0000-000000000023",
            requestedBy:    "10000000-0000-0000-0000-000000000015",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 4, 22),
            returnedDate:   null),
 
        // Assignment b024 | Asset NS000003 | assignedTo: user005 | assignedBy: admin003
        Build("c0000000-0000-0000-0000-000000000002",
            assignmentId:   "b0000000-0000-0000-0000-000000000024",
            requestedBy:    "10000000-0000-0000-0000-000000000005",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 5, 5),
            returnedDate:   null),
 
        // Assignment b025 | Asset LT000007 | assignedTo: user006 | assignedBy: admin001
        Build("c0000000-0000-0000-0000-000000000003",
            assignmentId:   "b0000000-0000-0000-0000-000000000025",
            requestedBy:    "10000000-0000-0000-0000-000000000006",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 5, 14),
            returnedDate:   null),
 
        // ════════════════════════════════════════════════════════════════════════
        // HA NOI – Completed  (Assignment: Returned)
        // ════════════════════════════════════════════════════════════════════════
 
        // Assignment b011 | Asset DC000001 | assignedTo: user004 | assignedBy: admin002
        Build("c0000000-0000-0000-0000-000000000004",
            assignmentId:   "b0000000-0000-0000-0000-000000000011",
            requestedBy:    "10000000-0000-0000-0000-000000000004",
            acceptedBy:     "10000000-0000-0000-0000-000000000002",
            state:          ReturnRequestState.Completed,
            createdDate:    new DateTime(2026, 5, 7),
            returnedDate:   new DateTime(2026, 5, 14)),
 
        // Assignment b012 | Asset NS000001 | assignedTo: user004 | assignedBy: admin003
        Build("c0000000-0000-0000-0000-000000000005",
            assignmentId:   "b0000000-0000-0000-0000-000000000012",
            requestedBy:    "10000000-0000-0000-0000-000000000004",
            acceptedBy:     "10000000-0000-0000-0000-000000000003",
            state:          ReturnRequestState.Completed,
            createdDate:    new DateTime(2026, 5, 12),
            returnedDate:   new DateTime(2026, 5, 19)),
 
        // ════════════════════════════════════════════════════════════════════════
        // HO CHI MINH – WaitingForReturning  (Assignment: Accepted + isReturning=true)
        // ════════════════════════════════════════════════════════════════════════
 
        // Assignment b034 | Asset LT000008 | assignedTo: lanlt(user019) | assignedBy: annh(admin016)
        Build("c0000000-0000-0000-0000-000000000006",
            assignmentId:   "b0000000-0000-0000-0000-000000000034",
            requestedBy:    "10000000-0000-0000-0000-000000000019",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 2, 7),
            returnedDate:   null),
 
        // Assignment b036 | Asset KB000008 | assignedTo: lanlt(user019) | assignedBy: kiettm(admin018)
        Build("c0000000-0000-0000-0000-000000000007",
            assignmentId:   "b0000000-0000-0000-0000-000000000036",
            requestedBy:    "10000000-0000-0000-0000-000000000019",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 3, 18),
            returnedDate:   null),
 
        // Assignment b039 | Asset MN000007 | assignedTo: mydb(user021) | assignedBy: trangpn(admin017)
        Build("c0000000-0000-0000-0000-000000000008",
            assignmentId:   "b0000000-0000-0000-0000-000000000039",
            requestedBy:    "10000000-0000-0000-0000-000000000021",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 3, 10),
            returnedDate:   null),
 
        // Assignment b041 | Asset MS000007 | assignedTo: nhiht(user023) | assignedBy: annh(admin016)
        Build("c0000000-0000-0000-0000-000000000009",
            assignmentId:   "b0000000-0000-0000-0000-000000000041",
            requestedBy:    "10000000-0000-0000-0000-000000000023",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 3, 20),
            returnedDate:   null),
 
        // Assignment b043 | Asset SC000007 | assignedTo: chaunm(user025) | assignedBy: kiettm(admin018)
        Build("c0000000-0000-0000-0000-000000000010",
            assignmentId:   "b0000000-0000-0000-0000-000000000043",
            requestedBy:    "10000000-0000-0000-0000-000000000025",
            acceptedBy:     null,
            state:          ReturnRequestState.WaitingForReturning,
            createdDate:    new DateTime(2026, 4, 5),
            returnedDate:   null),
 
        // ════════════════════════════════════════════════════════════════════════
        // HO CHI MINH – Completed  (Assignment: Returned)
        // ════════════════════════════════════════════════════════════════════════
 
        // Assignment b030 | Asset BM000006 | assignedTo: lanlt(user019) | assignedBy: annh(admin016)
        Build("c0000000-0000-0000-0000-000000000011",
            assignmentId:   "b0000000-0000-0000-0000-000000000030",
            requestedBy:    "10000000-0000-0000-0000-000000000019",
            acceptedBy:     "10000000-0000-0000-0000-000000000016",
            state:          ReturnRequestState.Completed,
            createdDate:    new DateTime(2026, 1, 18),
            returnedDate:   new DateTime(2026, 1, 25)),
 
        // Assignment b031 | Asset BM1000006 | assignedTo: lanlt(user019) | assignedBy: trangpn(admin017)
        Build("c0000000-0000-0000-0000-000000000012",
            assignmentId:   "b0000000-0000-0000-0000-000000000031",
            requestedBy:    "10000000-0000-0000-0000-000000000019",
            acceptedBy:     "10000000-0000-0000-0000-000000000017",
            state:          ReturnRequestState.Completed,
            createdDate:    new DateTime(2026, 2, 3),
            returnedDate:   new DateTime(2026, 2, 10)),
 
        // Assignment b032 | Asset PR000006 | assignedTo: lanlt(user019) | assignedBy: kiettm(admin018)
        Build("c0000000-0000-0000-0000-000000000013",
            assignmentId:   "b0000000-0000-0000-0000-000000000032",
            requestedBy:    "10000000-0000-0000-0000-000000000019",
            acceptedBy:     "10000000-0000-0000-0000-000000000018",
            state:          ReturnRequestState.Completed,
            createdDate:    new DateTime(2026, 2, 22),
            returnedDate:   new DateTime(2026, 3, 1)),
 
        // Assignment b033 | Asset SC000006 | assignedTo: lanlt(user019) | assignedBy: annh(admin016)
        Build("c0000000-0000-0000-0000-000000000014",
            assignmentId:   "b0000000-0000-0000-0000-000000000033",
            requestedBy:    "10000000-0000-0000-0000-000000000019",
            acceptedBy:     "10000000-0000-0000-0000-000000000016",
            state:          ReturnRequestState.Completed,
            createdDate:    new DateTime(2026, 3, 12),
            returnedDate:   new DateTime(2026, 3, 19)),
    };

        private static ReturnRequest Build(
            string id,
            string assignmentId,
            string requestedBy,
            string? acceptedBy,
            ReturnRequestState state,
            DateTime createdDate,
            DateTime? returnedDate)
        {
            var utcCreated = DateTime.SpecifyKind(createdDate, DateTimeKind.Utc);
            var utcReturned = returnedDate.HasValue
                ? DateTime.SpecifyKind(returnedDate.Value, DateTimeKind.Utc)
                : (DateTime?)null;

            return new ReturnRequestBuilder()
                .WithId(Guid.Parse(id))
                .WithAssignmentId(Guid.Parse(assignmentId))
                .WithRequestedByUserId(Guid.Parse(requestedBy))
                .WithAcceptedByUserId(acceptedBy is null ? null : Guid.Parse(acceptedBy))
                .WithState(state)
                .WithReturnedAtUtc(utcReturned)
                .WithCreatedAtUtc(utcCreated)
                .WithUpdatedAtUtc(utcReturned)
                .Build();
        }
    }
}
