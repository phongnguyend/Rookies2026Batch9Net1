import { SortDirection } from "@/lib/api/base.types";
import type { ReactNode } from "react";

export interface ColumnDef<T> {
  key: string;
  header: ReactNode;
  render?: (row: T, index: number) => ReactNode;
  className?: string;
  sortable?: boolean;
  headerTestId?: string;
  cellTestId?: (row: T, index: number) => string;
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

export default function AssignmentTable<T>({
  data,
  columns,
  isLoading = false,
  emptyMessage = "No records found.",
  onRowClick,
  sorts = [],
  onSortChange,
  getRowKey,
  rowTestId
}: DataTableProps<T>) {
  const handleSort = (key: string) => {
    const currentSort = sorts.find((s) => s.key === key);

    let nextSorts: SortItem[] = [];

    if (!currentSort) {
      nextSorts = [
        {
          key,
          direction: SortDirection.Asc,
        },
      ];
    }

    else if (currentSort.direction === SortDirection.Asc) {
      nextSorts = [
        {
          key,
          direction: SortDirection.Desc,
        },
      ];
    }

    else {
      nextSorts = [];
    }

    onSortChange?.(nextSorts);
  };

  const getSortIcon = (direction?: SortDirection) => {
    switch (direction) {
      case SortDirection.Asc:
        return "↑";

      case SortDirection.Desc:
        return "↓";

      default:
        return "↕";
    }
  };

  return (
    <div className="w-full overflow-x-auto rounded border border-gray-200">
      <table className="w-full min-w-[600px] text-left text-sm table-fixed">
        <thead>
          <tr className="border-b border-gray-400 bg-gray-50">
            {columns.map((column) => {
              const currentSort = sorts.find(
                (s) => s.key === column.key
              );

              return (
                <th
                  key={String(column.key)}
                  onClick={() =>
                    column.sortable &&
                    handleSort(String(column.key))
                  }
                  className={`
                    px-3 py-2 sm:px-4 sm:py-3 font-semibold text-xs sm:text-sm
                    ${column.className ?? ""}
                    ${column.sortable
                      ? "cursor-pointer select-none hover:text-primary transition-colors"
                      : ""
                    }
                  `}
                  data-testid={column.headerTestId}  //For column header in table
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
              <td
                colSpan={columns.length}
                className="py-8 text-center text-sm"
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
                    data-testid={column.cellTestId?.(row, rowIndex)} //For colum value in table
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
