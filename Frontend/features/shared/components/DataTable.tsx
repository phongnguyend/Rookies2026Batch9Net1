import type { ReactNode } from "react";

export interface ColumnDef<T> {
  key: string;
  header: ReactNode;
  render?: (row: T, index: number) => ReactNode;
  className?: string;
  sortable?: boolean;
}

type SortDirection = "asc" | "desc";

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
}

export default function DataTable<T>({
  data,
  columns,
  isLoading = false,
  onRowClick,
  emptyMessage = "No records found.",
  sorts = [],
  onSortChange,
}: DataTableProps<T>) {
  const handleSort = (key: string) => {
    const currentSort = sorts.find((s) => s.key === key);

    let nextSorts: SortItem[];

    if (!currentSort) {
      nextSorts = [...sorts, { key, direction: "asc" }];
    } else if (currentSort.direction === "asc") {
      nextSorts = sorts.map((s) =>
        s.key === key ? { ...s, direction: "desc" } : s
      );
    } else {
      nextSorts = sorts.filter((s) => s.key !== key);
    }

    onSortChange?.(nextSorts);
  };
  return (
    <div className="w-full overflow-x-auto">
      <table className="w-full text-left text-sm">
        <thead>
          <tr className="border-b border-gray-400">
            {columns.map((column) => {
              const currentSort = sorts.find((s) => s.key === column.key);
              const sortIndex = sorts.findIndex((s) => s.key === column.key);

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
                      {currentSort
                        ? currentSort.direction === "asc"
                          ? "↑"
                          : "↓"
                        : "↕"}
                    </span>
                  )}

                  {currentSort && (
                    <span className="ml-1 text-xs text-gray-400">
                      {sortIndex + 1}
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
