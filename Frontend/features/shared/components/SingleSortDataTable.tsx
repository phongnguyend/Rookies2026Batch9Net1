import { SortDirection } from "@/lib/api/base.types";
import type { ReactNode } from "react";

export interface ColumnDef<T> {
  key: string;
  header: ReactNode;
  render?: (row: T, index: number) => ReactNode;
  className?: string;
  sortable?: boolean;
  headerTestId?: string;
  cellTestId?: string | ((row: T, index: number) => string);
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

  // sort
  sorts?: SortItem[];
  onSortChange?: (sorts: SortItem[]) => void;
  getRowKey?: (row: T, index: number) => React.Key;
  rowTestId?: (row: T, index: number) => string;
}

export default function SingleSortDataTable<T>({
  data,
  columns,
  isLoading = false,
  onRowClick,
  emptyMessage = "No records found.",
  sorts = [],
  onSortChange,
  getRowKey,
  rowTestId
}: DataTableProps<T>) {
  const handleSort = (key: string) => {
    const currentSort = sorts.find((s) => s.key === key);

    let nextSorts: SortItem[];

    if (!currentSort) {
      // New column selected — reset all others, sort this one Asc
      nextSorts = [{ key, direction: SortDirection.Asc }];
    } else if (currentSort.direction === SortDirection.Asc) {
      // Same column, cycle to Desc
      nextSorts = [{ key, direction: SortDirection.Desc }];
    } else {
      // Same column on Desc — clear sort
      nextSorts = [];
    }

    onSortChange?.(nextSorts);
  };

  const getSortIcon = (direction?: SortDirection) => {
    if (direction === SortDirection.Asc) return "↑";
    if (direction === SortDirection.Desc) return "↓";
    return "↕";
  };

  // if (isLoading) {
  //   return (
  //     <div className="text-center">
  //       <span className="loading loading-spinner loading-lg text-primary"></span>
  //     </div>
  //   );
  // }

  // if (data.length === 0) {
  //   return <p className="py-8 text-center text-gray-500">{emptyMessage}</p>;
  // }

  return (
    <div className="w-full overflow-x-auto rounded border border-gray-200">
      <table className="w-full min-w-[600px] text-left text-sm table-fixed">
        <thead>
          <tr className="border-b border-gray-400 bg-gray-50">
            {columns.map((column) => {
              const currentSort = sorts.find((s) => s.key === column.key);

              return (
                <th
                  key={column.key}
                  data-testid={column.headerTestId}
                  onClick={() => column.sortable && handleSort(column.key)}
                  className={`px-3 py-2 sm:px-4 sm:py-3 font-semibold text-xs sm:text-sm
                    ${column.className ?? ""}
                    ${column.sortable
                      ? "cursor-pointer select-none hover:text-primary transition-colors"
                      : ""
                    }`}
                >
                  <div className="inline-flex items-center gap-1">
                    {column.header}

                    {column.sortable && (
                      <span className="text-xs">
                        {getSortIcon(currentSort?.direction)}
                      </span>
                    )}
                  </div>
                </th>
              );
            })}
          </tr>
        </thead>

        <tbody>
          {isLoading ? (
            <tr>
              <td colSpan={columns.length} className="py-8 text-center text-sm">
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
                key={
                  getRowKey
                    ? getRowKey(row, rowIndex)
                    : rowIndex
                }
                onClick={() => onRowClick?.(row)}
                className={`
                  border-b border-gray-200
                  ${onRowClick
                    ? "cursor-pointer hover:bg-gray-50 transition-colors"
                    : ""
                  }
                `}
                data-testid={rowTestId?.(row, rowIndex)}   //For assignment detail
              >
                {columns.map((column) => (
                  <td
                    key={String(column.key)}
                    data-testid={
                      typeof column.cellTestId === "function"
                        ? column.cellTestId(row, rowIndex)
                        : column.cellTestId
                    } //For colum value in table
                    className={`px-3 py-2 sm:px-4 sm:py-3 text-xs sm:text-sm truncate ${column.className ?? ""}`}
                  >
                    {column.render
                      ? column.render(row, rowIndex)
                      : (row as Record<string, ReactNode>)[
                      String(column.key)
                      ]}
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
