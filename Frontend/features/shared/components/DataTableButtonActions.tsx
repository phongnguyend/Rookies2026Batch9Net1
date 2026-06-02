import { ReactNode } from "react";

interface DataTableButtonActionsProps<T> {
  row: T;
  onAccept?: (row: T) => void;
  onDecline?: (row: T) => void;
  onReturn?: (row: T) => void;
  disabledAccept?: boolean;
  disabledDecline?: boolean;
  disabledReturn?: boolean;
  acceptBtnTestId?: string;
  declineBtnTestId?: string;
  returnBtnTestId?: string;
  acceptIcon?: ReactNode;
  declineIcon?: ReactNode;
  returnIcon?: ReactNode;
}

export default function DataTableButtonActions<T>({
  row,
  onAccept,
  onDecline,
  onReturn,
  disabledAccept = false,
  disabledDecline = false,
  disabledReturn = false,
  acceptBtnTestId = "btnAcceptAssignment",
  declineBtnTestId = "btnDeclineAssignment",
  returnBtnTestId = "btnReturnAssignment",
  acceptIcon = "✓",
  declineIcon = "⊗",
  returnIcon = "↻",
}: DataTableButtonActionsProps<T>) {
  return (
    <div className="flex items-center gap-3">
      {onAccept && (
        <button
          type="button"
          disabled={disabledAccept}
          onClick={(e) => {
            e.stopPropagation();
            onAccept(row);
          }}
          data-testid={acceptBtnTestId}
          className="text-green-600 disabled:cursor-not-allowed disabled:opacity-30 cursor-pointer"
          title="Accept"
        >
          {acceptIcon}
        </button>
      )}

      {onDecline && (
        <button
          type="button"
          disabled={disabledDecline}
          onClick={(e) => {
            e.stopPropagation();
            onDecline(row);
          }}
          data-testid={declineBtnTestId}
          className="text-red-400 disabled:cursor-not-allowed disabled:opacity-30 cursor-pointer"
          title="Decline"
        >
          {declineIcon}
        </button>
      )}

      {onReturn && (
        <button
          type="button"
          disabled={disabledReturn}
          onClick={(e) => {
            e.stopPropagation();
            onReturn(row);
          }}
          data-testid={returnBtnTestId}
          className="text-blue-600 disabled:cursor-not-allowed disabled:opacity-30 cursor-pointer"
          title="Return"
        >
          {returnIcon}
        </button>
      )}
    </div>
  );
}
