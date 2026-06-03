"use client";

import { LookupAssetsSummary } from "@/features/Assets/assets.types";
import { useAssetsWithAssignedLookupTableState } from "./useAssetsWithAssignedLookupTableState";
import SingleSortDataTable, { SortItem } from "../../SingleSortDataTable";
import { SortDirection } from "@/lib/api/base.types";
import { useLookupAssetsWithAssignedQuery } from "@/features/Assets/assets.api";
import { useMemo, useState } from "react";
import { createColumns } from "./columns";
import SearchInput from "../../SearchInput";
import { Search } from "lucide-react";
import Pagination from "../../Pagination";

export interface AssetsWithAssignedLookupTableProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (asset: LookupAssetsSummary | null) => void;
  onCancel?: () => void;
  pendingAsset: LookupAssetsSummary | null;
  onPendingAssetChange: (asset: LookupAssetsSummary | null) => void;
  assignedAssetId?: string;
}

export const AssetsWithAssignedLookupTable = ({
  isOpen,
  onClose,
  onConfirm,
  onCancel,
  pendingAsset,
  onPendingAssetChange,
  assignedAssetId,
}: AssetsWithAssignedLookupTableProps) => {
  const { params, dispatch } =
    useAssetsWithAssignedLookupTableState(assignedAssetId);
  const sorts: SortItem[] = params.sortBy
    ? [
        {
          key: params.sortBy,
          direction: params.sortDesc ? SortDirection.Desc : SortDirection.Asc,
        },
      ]
    : [];
  const { data, isLoading } = useLookupAssetsWithAssignedQuery(params);
  const columns = useMemo(
    () => createColumns(pendingAsset, onPendingAssetChange),
    [pendingAsset, onPendingAssetChange],
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
              Select Asset
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
                  txtInputTestId="txtSearchAsset"
                  btnSearchTestId="btnSearchAsset"
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
            onRowClick={onPendingAssetChange}
            emptyMessage="No assets found."
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
              data-testid={"btnSaveAsset"}
              onClick={() => onConfirm(pendingAsset)}
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
              data-testid={"btnCancelAsset"}
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
