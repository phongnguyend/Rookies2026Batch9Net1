using NashAssetManagement.Domain.Entities.Core;
using NashAssetManagement.Domain.Enums;

namespace NashAssetManagement.Persistence.Builder
{
    public sealed class ReturnRequestBuilder
    {
        private Guid _id = Guid.NewGuid();
        private DateTime? _returnedAtUtc;
        private ReturnRequestState _state = ReturnRequestState.WaitingForReturning;
        private Guid _assignmentId = Guid.NewGuid();
        private Guid _requestedByUserId = Guid.NewGuid();
        private Guid? _acceptedByUserId;
        private DateTime _createdAtUtc = DateTime.UtcNow;
        private DateTime? _updatedAtUtc;

        public ReturnRequestBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public ReturnRequestBuilder WithReturnedAtUtc(DateTime? returnedAtUtc)
        {
            _returnedAtUtc = returnedAtUtc;
            return this;
        }

        public ReturnRequestBuilder WithState(ReturnRequestState state)
        {
            _state = state;
            return this;
        }

        public ReturnRequestBuilder WithAssignmentId(Guid assignmentId)
        {
            _assignmentId = assignmentId;
            return this;
        }

        public ReturnRequestBuilder WithRequestedByUserId(Guid requestedByUserId)
        {
            _requestedByUserId = requestedByUserId;
            return this;
        }

        public ReturnRequestBuilder WithAcceptedByUserId(Guid? acceptedByUserId)
        {
            _acceptedByUserId = acceptedByUserId;
            return this;
        }

        public ReturnRequestBuilder WithCreatedAtUtc(DateTime createdAtUtc)
        {
            _createdAtUtc = createdAtUtc;
            return this;
        }

        public ReturnRequestBuilder WithUpdatedAtUtc(DateTime? updatedAtUtc)
        {
            _updatedAtUtc = updatedAtUtc;
            return this;
        }

        public ReturnRequest Build()
        {
            return new ReturnRequest
            {
                Id = _id,
                ReturnedAtUtc = _returnedAtUtc,
                State = _state,
                AssignmentId = _assignmentId,
                RequestedByUserId = _requestedByUserId,
                AcceptedByUserId = _acceptedByUserId,
                CreatedAtUtc = _createdAtUtc,
                UpdatedAtUtc = _updatedAtUtc,
            };
        }
    }
}
