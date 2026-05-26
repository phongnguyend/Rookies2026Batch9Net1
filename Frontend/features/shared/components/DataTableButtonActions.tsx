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
}

export default function DataTableButtonActions<T>({
  row,
  onAccept,
  onDecline,
  onReturn,
  disabledAccept = false,
  disabledDecline = false,
  disabledReturn = false,
  acceptBtnTestId,
  declineBtnTestId,
  returnBtnTestId,
}: DataTableButtonActionsProps<T>) {
  return (
    <div className="flex items-center gap-3">
      {onAccept && (
        <button
          data-testid="btnAcceptAssignment"
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
          ✓
        </button>
      )}

      {onDecline && (
        <button
          data-testid="btnDeclineAssignment"
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
          ⊗
        </button>
      )}

      {onReturn && (
        <button
          data-testid="btnReturnAssignment"
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
          ↻
        </button>
      )}
    </div>
  );
}
