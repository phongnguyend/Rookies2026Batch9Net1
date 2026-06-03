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
        <div className="space-y-4">
          <div className="flex flex-col md:flex-row md:items-center">
            <div className="mb-6 text-xl font-bold text-primary md:mb-0">
              Select User
            </div>
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center lg:ml-auto">
              <div className="w-full sm:w-auto">
                <SearchInput
                  value={searchTerm}
                  placeholder="Search..."
                  onChange={setSearchTerm}
                  onSearch={(value) => {
                    dispatch({
                      type: "SET_SEARCH",
                      payload: { searchTerm: value },
                    });
                  }}
                  txtInputTestId="txtSearchUser"
                  btnSearchTestId="btnSearchUser"
                  searchIcon={<Search size={16} />}
                />
              </div>
            </div>
          </div>

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

          {/* Modal Actions */}
          <div className="px-6 py-4 bg-white flex items-center justify-end gap-3">
            <button
              type="button"
              data-testid={"btnSaveUser"}
              onClick={() => onConfirm(pendingUser)}
              disabled={isLoading}
              className="px-4 py-2 bg-primary hover:bg-primary/90 hover:cursor-pointer text-white font-semibold rounded flex items-center gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading && (
                <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              )}
              Save
            </button>

            <button
              type="button"
              data-testid={"btnCancelUser"}
              onClick={onClose}
              disabled={isLoading}
              className={
                onCancel
                  ? "px-4 py-2 bg-primary hover:bg-primary hover:cursor-pointer text-white font-semibold rounded flex items-center gap-2 shadow-sm transition-all duration-150 hover:shadow disabled:opacity-50 disabled:cursor-not-allowed"
                  : "px-4 py-2 border border-gray-400 rounded text-neutral-700 font-semibold hover:bg-gray-100 hover:cursor-pointer transition-colors duration-150 disabled:opacity-50 disabled:cursor-not-allowed"
              }
            >
              {onCancel && isLoading && (
                <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              )}
              Cancel
            </button>
          </div>
        </div>
      )}
    </>
  );
};
