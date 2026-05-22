using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Persistence.Builder
{
    public sealed class AssignmentBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _assetId;
        private Guid _assignedToUserId;
        private Guid _assignedByUserId;
        private DateTime _assignedDateAtUtc = DateTime.UtcNow;
        private string _note = "Note Sample";
        private AssignmentState _state = AssignmentState.WaitingForAcceptance;
        private bool _isReturning = false;
        private DateTime _createdAtUtc = DateTime.UtcNow;
        private DateTime? _updatedAtUtc;

        public AssignmentBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public AssignmentBuilder WithAssetId(Guid assetId)
        {
            _assetId = assetId;
            return this;
        }

        public AssignmentBuilder WithAssignedToUserId(Guid assignedToUserId)
        {
            _assignedToUserId = assignedToUserId;
            return this;
        }

        public AssignmentBuilder WithAssignedByUserId(Guid assignedByUserId)
        {
            _assignedByUserId = assignedByUserId;
            return this;
        }

        public AssignmentBuilder WithAssignedDateAtUtc(DateTime assignedDateAtUtc)
        {
            _assignedDateAtUtc = assignedDateAtUtc;
            return this;
        }

        public AssignmentBuilder WithNote(string note)
        {
            _note = note;
            return this;
        }

        public AssignmentBuilder WithState(AssignmentState state)
        {
            _state = state;
            return this;
        }

        public AssignmentBuilder WithIsReturning(bool isReturning)
        {
            _isReturning = isReturning;
            return this;
        }

        public AssignmentBuilder WithCreatedAtUtc(DateTime createdAtUtc)
        {
            _createdAtUtc = createdAtUtc;
            return this;
        }

        public AssignmentBuilder WithUpdatedAtUtc(DateTime? updatedAtUtc)
        {
            _updatedAtUtc = updatedAtUtc;
            return this;
        }

        public Assignment Build()
        {
            return new Assignment
            {
                Id = _id,
                AssetId = _assetId,
                AssignedToUserId = _assignedToUserId,
                AssignedByUserId = _assignedByUserId,
                AssignedAtUtc= _assignedDateAtUtc,
                Note = _note,
                State = _state,
                IsReturning = _isReturning,
                CreatedAtUtc = _createdAtUtc,
                UpdatedAtUtc = _updatedAtUtc,
            };
        }
    }
}
