import { SortDirection } from "@/lib/api/base.types";
import type { ReactNode } from "react";

export interface ColumnDef<T> {
  key: string;
  header: ReactNode;
  render?: (row: T, index: number) => ReactNode;
  className?: string;
  sortable?: boolean;
  TitleColumnTestId?: string;
  ColumnTestId?: (row: T, index: number) => string;
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
    <div className="relative z-0 w-full overflow-x-auto">
      <table className="w-full text-left text-sm">
        <thead>
          <tr className="border-b border-gray-400">
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
                    py-2 font-semibold whitespace-nowrap
                    ${column.className ?? ""}
                    ${column.sortable
                      ? "cursor-pointer select-none hover:text-primary transition-colors"
                      : ""
                    }
                  `}
                  data-testid={column.TitleColumnTestId}  //For column header in table
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
                    data-testid={column.ColumnTestId?.(row, rowIndex)} //For colum value in table
                    className={`py-3 ${column.className ?? ""}`}
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
