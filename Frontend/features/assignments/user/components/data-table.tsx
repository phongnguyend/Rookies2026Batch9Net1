"use client";

import { SortItem } from "@/features/shared/components/DataTable";
import { columns } from "./columns";
import { userAssignmentApi } from "../user-assignment.api";
import { useUserAssignmentTableState } from "./useUserAssignmentsTableState";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import SingleSortDataTable from "@/features/shared/components/SingleSortDataTable";

export const UserAssignmentsDataTable = () => {
  const { params, dispatch } = useUserAssignmentTableState();
  // Derive sorts for DataTable directly from params — single source of truth
  const sorts: SortItem[] = params.sortBy
    ? [
        {
          key: params.sortBy,
          direction: params.sortDesc ? SortDirection.Desc : SortDirection.Asc,
        },
      ]
    : [];
  const { data, isLoading } =
    userAssignmentApi.useViewUserAssignmentsQuery(params);

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

  return (
    <div className="space-y-4">
      <SingleSortDataTable
        data={data?.items ?? []}
        columns={columns}
        isLoading={isLoading}
        sorts={sorts}
        onSortChange={handleSortChange}
        emptyMessage="No assignments found."
      />

      {/* Pagination */}
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
  );
};
