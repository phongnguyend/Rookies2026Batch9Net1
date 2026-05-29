import { formatDate } from "@/utils/datetime.utils";
import { ViewUserAssignments } from "../user-assignment.types";
import { userAssignmentApi } from "../user-assignment.api";
import { displayAssignmentState } from "@/utils/assignment.utils";
import { AssignmentState } from "@/lib/api/base.types";

interface Props {
  assignment: ViewUserAssignments.UserAssignmentSummary | null;
  onClose: () => void;
  "data-testid"?: string;
}

const isAcceptedAndReturning = (
  assignment: ViewUserAssignments.UserAssignmentSummary,
): boolean => {
  return (
    assignment.state === AssignmentState.Accepted && assignment.isReturning
  );
};


const Field = ({ label, value }: { label: string; value?: string }) => (
  <div data-testid="dgdAssignmentRow" className="flex gap-4">
    <span className="w-24 shrink-0 text-gray-500">{label}</span>
    <span className="wrap-break-word">{value ?? "—"}</span>
  </div>
);

export default function UserAssignmentDetailDialog({
  assignment,
  onClose,
  "data-testid": testId,
}: Props) {
  const isOpen = !!assignment;

  const { data, isFetching } =
    userAssignmentApi.useViewUserAssignmentDetailQuery(
      { assignmentId: assignment?.id ?? "" },
      { skip: !assignment }, // don't fetch when no row is selected
    );

  if (!isOpen) return null;

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
      onClick={onClose} // click backdrop to close
    >
      <div
        data-testid={testId}
        className="relative w-full max-w-xl overflow-hidden rounded-lg bg-white shadow-lg border"
        onClick={(e) => e.stopPropagation()} // prevent backdrop click from firing inside
      >
        {/* Header */}
        <div className="flex items-center justify-between border-b border-gray-300 bg-gray-200 px-10 py-4">
          <h2 className="text-lg font-semibold text-primary">
            Detailed Assignment Information
          </h2>
          <button
            data-testid={"btnClose"}
            onClick={onClose}
            className="rounded border-3 font-semibold border-primary px-2 py-0.5 text-sm text-primary hover:cursor-pointer hover:font-bold"
          >
            ✕
          </button>
        </div>

        {/* Body */}
        <div className="px-10 py-4">
          {isFetching ? (
            <div className="py-8 text-center text-gray-400">Loading...</div>
          ) : (
            <div className="space-y-3 text-sm">
              <Field label="Asset Code" value={data?.assetCode} />
              <Field label="Asset Name" value={data?.assetName} />
              <Field label="Specification" value={data?.specification} />
              <Field label="Assigned to" value={data?.assigneeName} />
              <Field label="Assigned by" value={data?.assignerName} />
              <Field
                label="Assigned Date"
                value={
                  data?.assignedDate ? formatDate(data.assignedDate) : undefined
                }
              />
              <Field
                label="State"
                value={displayAssignmentState(data?.state!)}
              />
              <Field label="Note" value={data?.note} />
              {isAcceptedAndReturning(assignment) && <div className="text-center my-2 text-gray-500">
                Assignment is currently in returning process
              </div>}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
