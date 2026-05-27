interface DataTableButtonActionsProps<T> {
  row: T;
  onAccept?: (row: T) => void;
  onDecline?: (row: T) => void;
  onReturn?: (row: T) => void;
  disabledAccept?: boolean;
  disabledDecline?: boolean;
  disabledReturn?: boolean;
}

export default function DataTableButtonActions<T>({
  row,
  onAccept,
  onDecline,
  onReturn,
  disabledAccept = false,
  disabledDecline = false,
  disabledReturn = false,
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
          className="text-green-600 disabled:cursor-not-allowed disabled:opacity-30"
          title="Accept"
        >
          {"\u2713"}
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
          className="text-red-400 disabled:cursor-not-allowed disabled:opacity-30"
          title="Decline"
        >
          {"\u2297"}
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
          className="text-blue-600 disabled:cursor-not-allowed disabled:opacity-30"
          title="Return"
        >
          {"\u21bb"}
        </button>
      )}
    </div>
  );
}
