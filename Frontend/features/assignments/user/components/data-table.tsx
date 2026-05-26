"use client";

import { SortItem } from "@/features/shared/components/DataTable";
import { createColumns } from "./columns";
import { userAssignmentApi } from "../user-assignment.api";
import { useUserAssignmentTableState } from "./useUserAssignmentsTableState";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import SingleSortDataTable from "@/features/shared/components/SingleSortDataTable";
import { ViewUserAssignments } from "../user-assignment.types";
import { useMemo, useState } from "react";
import UserAssignmentDetailDialog from "./UserAssignmentDetailDialog";
import ConfirmModal from "@/features/shared/components/Modal/ConfirmModal";
import { useDispatch } from "react-redux";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";

export const UserAssignmentsDataTable = () => {
  const { params, dispatch } = useUserAssignmentTableState();

  const dispatchAction = useDispatch();

  const [selectedAssignment, setSelectedAssignment] =
    useState<ViewUserAssignments.UserAssignmentSummary | null>(null);

  const [returningAssignment, setReturningAssignment] =
    useState<ViewUserAssignments.UserAssignmentSummary | null>(null);

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

  const [createReturnRequest, { isLoading: isReturning }] =
    userAssignmentApi.useUserCreateReturnRequestMutation();

  const columns = useMemo(
    () => createColumns({ onReturnClick: setReturningAssignment }),
    [],
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

  const handleConfirmReturn = async () => {
    if (!returningAssignment) return;

    try {
      await createReturnRequest({
        assignmentId: returningAssignment.id,
      }).unwrap();

      setReturningAssignment(null);
      dispatchAction(
        enqueueToast({
          message: "Return request created successfully.",
          type: ToastType.Success,
        }),
      );
    } catch {
      setReturningAssignment(null);
      dispatchAction(
        enqueueToast({
          message: "Failed to create return request. Please try again.",
          type: ToastType.Error,
        }),
      );
    }
  };

  return (
    <div className="space-y-4">
      <SingleSortDataTable
        data-testid={"test"}
        data={data?.items ?? []}
        columns={columns}
        isLoading={isLoading}
        sorts={sorts}
        onSortChange={handleSortChange}
        onRowClick={setSelectedAssignment}
        emptyMessage="No assignments found."
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

      <UserAssignmentDetailDialog
        assignment={selectedAssignment}
        onClose={() => setSelectedAssignment(null)}
      />

      <ConfirmModal
        isOpen={!!returningAssignment}
        onClose={() => setReturningAssignment(null)}
        onYes={handleConfirmReturn}
        isLoading={isReturning}
        title="Are you sure?"
        body={<p>Do you want to create a returning request for asset?</p>}
        yesButtonLabel="Yes"
        noButtonLabel="No"
      />
    </div>
  );
};
