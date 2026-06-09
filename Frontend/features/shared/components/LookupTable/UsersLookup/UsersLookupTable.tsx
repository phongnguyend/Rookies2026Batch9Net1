"use client";

import { useMemo, useState } from "react";
import { useUsersLookupTableState } from "./useUsersLookupTableState";
import { LookupUsers } from "@/features/users/users.types";
import SingleSortDataTable, { SortItem } from "../../SingleSortDataTable";
import { SortDirection } from "@/lib/api/base.types";
import { useLookupUsersQuery } from "@/features/users/users.api";
import { createColumns } from "./columns";
import Pagination from "../../Pagination";
import SearchInput from "../../SearchInput";
import { Search } from "lucide-react";

export interface UsersLookupTableProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (user: LookupUsers.LookupUsersSummary | null) => void;
  onCancel?: () => void;
  pendingUser: LookupUsers.LookupUsersSummary | null;
  onPendingUserChange: (user: LookupUsers.LookupUsersSummary | null) => void;
}

export const UsersLookupTable = ({
  isOpen,
  onClose,
  onConfirm,
  onCancel,
  pendingUser,
  onPendingUserChange,
}: UsersLookupTableProps) => {
  const { params, dispatch } = useUsersLookupTableState();
  const sorts: SortItem[] = params.sortBy
    ? [
      {
        key: params.sortBy,
        direction: params.sortDesc ? SortDirection.Desc : SortDirection.Asc,
      },
    ]
    : [];
  const { data, isLoading } = useLookupUsersQuery(params);
  const columns = useMemo(
    () => createColumns(pendingUser, onPendingUserChange),
    [pendingUser, onPendingUserChange],
  );

  const handleSortChange = (nextSorts: SortItem[]) => {
    if (nextSorts.length === 0) {
      dispatch({ type: "CLEAR_SORTING" });
    } else {
      const { key, direction } = nextSorts[0];
      dispatch({
        type: "SET_SORTING",
        payload: { sortBy: key, sortDesc: direction === SortDirection.Desc },
      });
    }
  };

  const [searchTerm, setSearchTerm] = useState(params.searchTerm ?? "");

  return (
    <>
      {isOpen && (
        <div className="flex flex-col min-h-0 flex-1">
          {/* Header (shrink-0, never scrolls away) */}
          <div className="shrink-0 px-4 sm:px-6 pt-5 pb-3">
            <div className="flex flex-col gap-4 md:flex-row md:items-center">
              <div className="text-xl font-bold text-primary">Select User</div>

              <div className="flex flex-col gap-3 sm:flex-row sm:items-center md:ml-auto">
                <div className="w-full">
                  <SearchInput
                    value={searchTerm}
                    placeholder="Search..."
                    onChange={setSearchTerm}
                    onSearch={(value) => {
                      dispatch({
                        type: "SET_SEARCH",
                        payload: { searchTerm: value.trim().replace(/\s+/g, " ") },
                      });
                    }}
                    txtInputTestId="txtSearchUser"
                    btnSearchTestId="btnSearchUser"
                    searchIcon={<Search size={16} />}
                  />
                </div>
              </div>
            </div>
          </div>

          {/* Scrollable zone (table + pagination) */}
          <div className="flex-1 min-h-0 overflow-y-auto px-4 sm:px-6 flex flex-col gap-4 pb-2">
            <div className="overflow-x-auto">
              <SingleSortDataTable
                data-testid={"test"}
                data={data?.items ?? []}
                columns={columns}
                isLoading={isLoading}
                sorts={sorts}
                onSortChange={handleSortChange}
                onRowClick={onPendingUserChange}
                emptyMessage="No users found."
              />
            </div>
            <Pagination
              pageNumber={data?.pageNumber ?? 1}
              totalPages={data?.totalPages ?? 1}
              pageSize={data?.pageSize}
              totalCount={data?.totalCount}
              hasPreviousPage={data?.hasPreviousPage ?? false}
              hasNextPage={data?.hasNextPage ?? false}
              onPageChange={(page) => {
                dispatch({
                  type: "SET_PAGINATION",
                  payload: {
                    pageNumber: page,
                    pageSize: params.pageSize!,
                  },
                });
              }}
            />
          </div>

          {/* Action buttons (shrink-0, always visible) */}
          <div className="shrink-0 px-4 sm:px-6 py-4 flex flex-col-reverse sm:flex-row sm:justify-end gap-3">
            <button
              type="button"
              data-testid="btnSaveUser"
              onClick={() => onConfirm(pendingUser)}
              disabled={isLoading}
              className="w-full sm:w-auto px-4 py-2 bg-primary hover:bg-primary/90 hover:cursor-pointer text-white font-semibold rounded gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading && (
                <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              )}
              Save
            </button>

            <button
              type="button"
              data-testid="btnCancelUser"
              onClick={onClose}
              disabled={isLoading}
              className="w-full sm:w-auto px-4 py-2 border border-gray-400 rounded text-neutral-700 font-semibold hover:bg-gray-100 hover:cursor-pointer transition-colors duration-150 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Cancel
            </button>
          </div>
        </div>
      )}
    </>
  );
};
