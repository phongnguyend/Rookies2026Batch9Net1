import { SortDirection } from "@/lib/api/base.types";
import type { ReactNode } from "react";

export interface ColumnDef<T> {
  key: string;
  header: ReactNode;
  render?: (row: T, index: number) => ReactNode;
  className?: string;
  sortable?: boolean;
}

export interface SortItem {
  key: string;
  direction: SortDirection;
}

interface DataTableProps<T> {
  data: T[];
  columns: ColumnDef<T>[];
  isLoading?: boolean;
  onRowClick?: (row: T) => void;
  emptyMessage?: string;

  // single sort only
  sort?: SortItem | null;
  onSortChange?: (sort: SortItem | null) => void;
}

export default function DataTable<T>({
  data,
  columns,
  isLoading = false,
  onRowClick,
  emptyMessage = "No records found.",
  sort,
  onSortChange,
}: DataTableProps<T>) {

  const handleSort = (key: string) => {
    // no current sort → ASC
    if (!sort || sort.key !== key) {
      onSortChange?.({
        key,
        direction: SortDirection.Asc,
      });

      return;
    }

    // ASC → DESC
    if (sort.direction === SortDirection.Asc) {
      onSortChange?.({
        key,
        direction: SortDirection.Desc,
      });

      return;
    }

    // DESC → clear
    onSortChange?.(null);
  };

  const getSortIcon = (columnKey: string) => {
    if (!sort || sort.key !== columnKey)
      return "↕";

    if (sort.direction === SortDirection.Asc)
      return "↑";

    return "↓";
  };

  return (
    <div className="w-full overflow-x-auto">
      <table className="w-full text-left text-sm">
        <thead>
          <tr className="border-b border-gray-400">
            {columns.map((column) => (
              <th
                key={column.key}
                onClick={() =>
                  column.sortable && handleSort(column.key)
                }
                className={`py-2 font-semibold ${column.className ?? ""} ${
                  column.sortable
                    ? "cursor-pointer select-none hover:text-primary"
                    : ""
                }`}
              >
                {column.header}

                {column.sortable && (
                  <span className="ml-1">
                    {getSortIcon(column.key)}
                  </span>
                )}
              </th>
            ))}
          </tr>
        </thead>

        <tbody>
          {isLoading ? (
            <tr>
              <td
                colSpan={columns.length}
                className="py-8 text-center"
              >
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
                  onRowClick
                    ? "cursor-pointer hover:bg-gray-50"
                    : ""
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
