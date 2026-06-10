import { useGetAssignmentByIdQuery } from "@/features/assignments/admin/assignments.api";
import { displayAssignmentState } from "@/utils/assignment.utils";
import { FocusTrap } from "focus-trap-react";
import { X } from "lucide-react";
import { ReactNode, useEffect } from "react";

function DetailRow({
  label,
  value,
  render,
}: {
  label: string;
  value?: string;
  render?: (value: string) => ReactNode;
}) {
  return (
    <div className="grid grid-cols-[140px_1fr] gap-4 py-2 text-sm">
      <div className="text-gray-700">{label}</div>

      <div className="text-gray-900 wrap-break-word">
        {value
          ? render
            ? render(value)
            : value
          : "-"}
      </div>
    </div>
  );
}

export interface AssignmentDetailPopupProps {
  id: string;
  onClose: () => void;
}

export default function AssignmentDetailPopup({
  id,
  onClose,
}: AssignmentDetailPopupProps) {
  const {
    data,
    isLoading,
    isError,
  } = useGetAssignmentByIdQuery(id);

  // Close modal on ESC
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        onClose();
      }
    };

    window.addEventListener("keydown", handleKeyDown);

    return () => {
      window.removeEventListener(
        "keydown",
        handleKeyDown
      );
    };
  }, [onClose]);

  return (
    <FocusTrap
      focusTrapOptions={{
        escapeDeactivates: false, // we handle Escape ourselves above
        allowOutsideClick: true,  // lets the backdrop click still close it
      }}
    >
      <div
        className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4"
        onClick={onClose}
      >
        <div
          className="w-full max-w-xl overflow-hidden rounded-md border border-gray-200 bg-white shadow-2xl"
          onClick={(e) => e.stopPropagation()}
          data-testid="dlgAssignmentDetail"
        >
          {/* Header */}
          <div className="flex items-center justify-between border-b border-gray-200 bg-gray-100 px-5 py-3">
            <h2 className="text-lg font-bold text-[#cf2338]">
              Detailed Assignment Information
            </h2>

            <button
              type="button"
              onClick={onClose}
              aria-label="Close"
              className="hover:cursor-pointer flex h-7 w-7 items-center justify-center rounded border-2 border-primary text-primary transition hover:bg-primary hover:text-white"
              data-testid="btnClose"
            >
              <X />
            </button>
          </div>

          {/* Body */}
          <div className="px-6 py-4" data-testid="dgdAssignmentRow">
            {isLoading ? (
              <div className="py-8 text-center">
                Loading...
              </div>
            ) : isError ? (
              <div className="py-8 text-center text-red-500">
                Failed to load assignment details.
              </div>
            ) : data ? (
              <div className="divide-y divide-gray-100">
                <DetailRow
                  label="Asset Code"
                  value={data.assetCode}
                />

                <DetailRow
                  label="Asset Name"
                  value={data.assetName}
                />

                <DetailRow
                  label="Specification"
                  value={data.specification}
                  render={(value) => (
                    <div className="max-h-24 overflow-y-auto whitespace-pre-wrap wrap-break-word pr-1">
                      {value}
                    </div>
                  )}
                />

                <DetailRow
                  label="Assigned to"
                  value={data.assignedTo}
                />

                <DetailRow
                  label="Assigned by"
                  value={data.assignedBy}
                />

                <DetailRow
                  label="Assigned Date"
                  value={data.assignedDate}
                />

                <DetailRow
                  label="State"
                  value={data.state}
                  render={(value) => displayAssignmentState(value)}
                />

                <DetailRow
                  label="Note"
                  value={data.note}
                  render={(value) => (
                    <div className="max-h-32 overflow-y-auto whitespace-pre-wrap wrap-break-word">
                      {value}
                    </div>
                  )}
                />

                {data.isReturning && (<div className="my-2 text-gray-500 text-center text-sm">
                  Assignment is currently in returning process
                </div>)}
              </div>
            ) : (
              <div className="py-8 text-center text-gray-500">
                No data found.
              </div>
            )}
          </div>
        </div>
      </div>
    </FocusTrap>
  );
}
