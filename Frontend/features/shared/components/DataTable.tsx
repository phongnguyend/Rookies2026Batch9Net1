import type { ReactNode } from "react";

export interface ColumnDef<T> {
  key: string;
  header: ReactNode;
  render?: (row: T, index: number) => ReactNode;
  className?: string;
  sortable?: boolean;
}

type SortDirection = "asc" | "desc";

interface DataTableProps<T> {
  data: T[];
  columns: ColumnDef<T>[];
  isLoading?: boolean;
  onRowClick?: (row: T) => void;
  emptyMessage?: string;

  // sort 
  sortKey?: string;
  sortDirection?: SortDirection;
  onSortChange?: (key: string, direction: SortDirection) => void;
}

export default function DataTable<T>({
  data,
  columns,
  isLoading = false,
  onRowClick,
  emptyMessage = "No records found.",
  sortKey,
  sortDirection = "asc",
  onSortChange,
}: DataTableProps<T>) {
  const handleSort = (key: string) => {
    const nextDirection: SortDirection =
      sortKey === key && sortDirection === "asc" ? "desc" : "asc";

    onSortChange?.(key, nextDirection);
  };
  return (
    <div className="w-full overflow-x-auto">
      <table className="w-full text-left text-sm">
        <thead>
          <tr className="border-b border-gray-400">
            {columns.map((column) => {
              const isSorted = sortKey === column.key;

              return (
                <th
                  key={column.key}
                  onClick={() => column.sortable && handleSort(column.key)}
                  className={`py-2 font-semibold ${column.className ?? ""} ${
                    column.sortable
                      ? "cursor-pointer select-none hover:text-primary"
                      : ""
                  }`}
                >
                  {column.header}

                  {column.sortable && (
                    <span className="ml-1">
                      {isSorted
                        ? sortDirection === "asc"
                          ? "↑"
                          : "↓"
                        : "↕"}
                    </span>
                  )}
                </th>
              );
            })}
          </tr>
        </thead>

        <tbody>
          {isLoading ? (
            <tr>
              <td colSpan={columns.length} className="py-8 text-center">
                Loading...
              </td>
            </tr>
          ) : data.length === 0 ? (
            <tr>
              <td
                colSpan={columns.length}
                className="py-8 text-center text-gray-500"
              >
                {emptyMessage}
              </td>
            </tr>
          ) : (
            data.map((row, rowIndex) => (
              <tr
                key={rowIndex}
                onClick={() => onRowClick?.(row)}
                className={`border-b border-gray-300 ${
                  onRowClick ? "cursor-pointer hover:bg-gray-50" : ""
                }`}
              >
                {columns.map((column) => (
                  <td
                    key={column.key}
                    className={`py-2 ${column.className ?? ""}`}
                  >
                    {column.render
                      ? column.render(row, rowIndex)
                      : (row as Record<string, ReactNode>)[column.key]}
                  </td>
                ))}
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
