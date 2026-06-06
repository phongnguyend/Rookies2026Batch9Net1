"use client";
import { useSearchParams, useRouter, usePathname } from "next/navigation";
import {
  SortItem,
} from "@/features/shared/components/DataTable";
import {
  useDeleteAssignmentMutation,
  useGetAllAssignmentsQuery,
} from "@/features/assignments/admin/assignments.api";
import { Assignment, AssignmentState } from "@/features/assignments/admin/assignments.types";
import SearchInput from "@/features/shared/components/SearchInput";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import { useState } from "react";
import AssignmentDetailPopup from "../../../../features/assignments/admin/components/AssignmentDetailPopup";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import { displayAssignmentState } from "@/utils/assignment.utils";
import SingleSortDataTable, { ColumnDef } from "@/features/shared/components/SingleSortDataTable";
import { Pencil, RotateCcw, Trash2, CircleX } from "lucide-react";
import ConfirmModal from "@/features/shared/components/Modal/ConfirmModal";
import { useDispatch } from "react-redux";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import type { ApiErrorResponse } from "@/lib/api/base.types";
import { returnsApi } from "@/features/returns/returns.api";

const limit = 10;

export default function AssignmentsPage() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const dispatchAction = useDispatch();

  // Read from Url
  const page = Number(searchParams.get("page")) || 1;
  const search = searchParams.get("search") || "";
  const [searchInput, setSearchInput] = useState(search);
  const allowedStates = [AssignmentState.Accepted, AssignmentState.WaitingForAcceptance];
  const states = searchParams.getAll("state").filter((s) =>
    allowedStates.includes(s as AssignmentState)
  );
  const assignedDateParam = searchParams.get("assignedDate");
  const assignedDate = assignedDateParam ? new Date(assignedDateParam) : null;
  const sortBy = searchParams.get("sortBy") || undefined;
  const sortDesc = searchParams.get("sortDesc") === "true";
  const [selectedAssignmentId, setSelectedAssignmentId] = useState<string | null>(null);
  const [deletingAssignment, setDeletingAssignment] =
    useState<Assignment | null>(null);

  const sorts: SortItem[] = sortBy
    ? [{ key: sortBy, direction: sortDesc ? SortDirection.Desc : SortDirection.Asc }]
    : [];

  // Function update URL
  const updateParams = (
    updates: Record<string, string | string[] | undefined>
  ) => {
    const params = new URLSearchParams(searchParams.toString());
    Object.entries(updates).forEach(([key, value]) => {
      params.delete(key);
      if (Array.isArray(value)) {
        value.forEach((v) => params.append(key, v));
      } else if (value) {
        params.set(key, value);
      }
    });
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  };

  const { data, isLoading } = useGetAllAssignmentsQuery({
    pageNumber: page,
    pageSize: limit,
    searchTerm: search,
    state: states.length > 0 ? states : undefined,
    assignedDate: assignedDate
      ? new Date(
        Date.UTC(
          assignedDate.getFullYear(),
          assignedDate.getMonth(),
          assignedDate.getDate()
        )
      ).toISOString()
      : undefined,
    sortBy: sortBy,
    sortDirection: sortDesc ? SortDirection.Desc : SortDirection.Asc,
    includeDeleted: false,
  });
  const [deleteAssignment, { isLoading: isDeleting }] =
    useDeleteAssignmentMutation();

  const assignments = data?.items ?? [];

  const handleConfirmDeleteAssignment = async () => {
    if (!deletingAssignment) return;

    try {
      await deleteAssignment({
        assignmentId: deletingAssignment.id,
      }).unwrap();

      setDeletingAssignment(null);
      dispatchAction(
        enqueueToast({
          message: "Assignment deleted successfully.",
          type: ToastType.Success,
          testId: "toastSuccess",
        })
      );
    } catch (error) {
      setDeletingAssignment(null);

      const apiError = error as ApiErrorResponse;
      const message =
        apiError.status === 400 && apiError.detail
          ? apiError.detail
          : "Failed to delete assignment. Please try again.";

      dispatchAction(
        enqueueToast({
          message,
          type: ToastType.Error,
          testId: "toastError",
        })
      );
    }
  };

  //Columns
  const columns: ColumnDef<Assignment>[] = [
    {
      key: "no",
      header: "No.",
      className: "w-16",
      render: (_assignment, index) => (page - 1) * limit + index + 1,
    },
    {
      key: "assetCode",
      header: "Asset Code",
      sortable: true,
      className: "w-32",
      headerTestId: "btnSortAssetCode",
      cellTestId: (_row, index) => `colAssetCode-${index}`,
    },
    {
      key: "assetName",
      header: "Asset Name",
      sortable: true,
      className: "w-48",
      headerTestId: "btnSortAssetName",
      cellTestId: (_row, index) => `colAssetName-${index}`,
    },
    {
      key: "assignedTo",
      header: "Assigned to",
      sortable: true,
      className: "w-36",
      headerTestId: "btnSortAssignedTo",
      cellTestId: (_row, index) => `colAssignedTo-${index}`,
    },
    {
      key: "assignedBy",
      header: "Assigned by",
      sortable: true,
      className: "w-36",
      headerTestId: "btnSortAssignedBy",
      cellTestId: (_row, index) => `colAssignedBy-${index}`,
    },
    {
      key: "assignedDate",
      header: "Assigned Date",
      sortable: true,
      className: "w-32",
      headerTestId: "btnSortAssignedDate",
      cellTestId: (_row, index) => `colAssignedDate-${index}`,
    },
    {
      key: "state",
      header: "State",
      sortable: true,
      className: "w-28",
      headerTestId: "btnSortState",
      cellTestId: (_row, index) => `colState-${index}`,
      render: (row) =>
        displayAssignmentState(row.state),
    },
    {
      key: "actions",
      header: "",
      className: "w-28",
      render: (assignment) => {
        const isWaiting =
          assignment.state === "WaitingForAcceptance";

        const isAccepted =
          assignment.state === "Accepted";

        const isReturning = assignment.isReturning;

        const isFinal =
          assignment.state === "Returned" ||
          assignment.state === "Declined";

        return (
          <DataTableButtonActions
            row={assignment}
            disabledAccept={isAccepted || isFinal}
            disabledDecline={!isWaiting}
            disabledReturn={isWaiting || isFinal || isReturning}
            onAccept={(row) => console.log("accept", row)}
            onDecline={setDeletingAssignment}
            onReturn={(row) => setReturningAssignment(row)} //return request
            acceptBtnTestId="btnAcceptAssignment"
            declineBtnTestId="btnDeleteAssignment"
            returnBtnTestId="btnReturnAssignment"
            acceptIcon={<Pencil className="text-gray-500" size={20} strokeWidth={3} />}
            declineIcon={<Trash2 size={20} strokeWidth={3} />}
            returnIcon={<RotateCcw size={20} strokeWidth={3} />}
          />
        );
      },
    }
  ];

  //create return request
  const [createReturnRequest, { isLoading: isReturning }] =
    returnsApi.useAdminCreateReturnRequestMutation();

  const [returningAssignment, setReturningAssignment] =
    useState<Assignment | null>(null);

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
          testId: "toastSuccess",
        }),
      );
    } catch (error) {
      setReturningAssignment(null);
      dispatchAction(
        enqueueToast({
          message: "Failed to create return request. Please try again.",
          type: ToastType.Error,
          testId: "toastError",
        }),
      );
    }
  };

  return (
    <div data-testid="mnuManageAssignment">
      <h1 className="text-primary font-bold text-xl mb-6">Assignment List</h1>

      <div className="mb-4 flex flex-col gap-3 lg:flex-row lg:items-center">

        {/* Left group: State + Assigned Date */}
        <div className="flex flex-wrap gap-3">
          <div data-testid="ddlState" className="w-full sm:w-auto">
            <DropdownFilter
              items={Object.values(AssignmentState).map((s) => ({
                key: s,
                label: displayAssignmentState(s),
              }))}
              values={states}
              placeholder="State"
              getKey={(item) => item.key}
              getLabel={(item) => item.label}
              onChange={(values) =>
                updateParams({
                  state: values,
                  page: "1",
                })
              }
              getTestIdAll={"chkStateAll"}
              getTestId={(item) => `chkState${item.key.replace(/\s+/g, "")}`}
            />
          </div>

          <div data-testid="dtpAssignedDate" className="w-full sm:w-auto">
            <DatePickerInput
              value={assignedDate}
              onChange={(date) =>
                updateParams({
                  assignedDate: date
                    ? new Date(
                      Date.UTC(
                        date.getFullYear(),
                        date.getMonth(),
                        date.getDate()
                      )
                    )
                      .toISOString()
                      .split("T")[0]
                    : undefined,
                  page: "1",
                })
              }
              placeholder="Assigned Date"
              width="w-full"
            />
          </div>
        </div>

        {/* Right group: Search + Create button */}
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center lg:ml-auto">
          <div className="w-full sm:w-auto">
            <SearchInput
              value={searchInput}
              placeholder="Search..."
              onChange={(value) => {
                setSearchInput(value);
              }}
              onSearch={(value) => {
                updateParams({
                  search: value || undefined,
                  page: "1",
                });
              }}
              txtInputTestId="txtSearchAssignment"
              btnSearchTestId="btnSearchAssignment"
            />
          </div>

          <button
            className="w-full sm:w-auto rounded bg-primary px-5 py-2 font-semibold text-white whitespace-nowrap text-sm sm:text-base cursor-pointer"
            data-testid="btnCreateNewAssignment"
            onClick={() => router.push("/admin/assignments/create")}
          >
            Create new assignment
          </button>
        </div>

      </div>

      <div className="space-y-4">
        <SingleSortDataTable<Assignment>
          data={assignments}
          columns={columns}
          isLoading={isLoading}
          emptyMessage="No assignments found."
          sorts={sorts}
          onSortChange={(newSorts) => {
            const sort = newSorts[0];
            updateParams({
              sortBy: sort?.key,
              sortDesc:
                sort?.direction === SortDirection.Desc ? "true" : undefined,
              page: "1",
            });
          }}
          onRowClick={(row) => {
            setSelectedAssignmentId(row.id);
          }}
          rowTestId={(_row, index) => `dgdAssignmentRow-${index}`}
        />

        {selectedAssignmentId && (
          <AssignmentDetailPopup
            id={selectedAssignmentId}
            onClose={() => setSelectedAssignmentId(null)}
          />
        )}

        <ConfirmModal
          isOpen={!!deletingAssignment}
          onClose={() => setDeletingAssignment(null)}
          onYes={handleConfirmDeleteAssignment}
          isLoading={isDeleting}
          title="Are you sure?"
          body={<p>Do you want to delete this assignment?</p>}
          yesButtonLabel="Delete"
          noButtonLabel="Cancel"
          confirmBtnTestId="btnDelete"
          cancelBtnTestId="btnCancel"
        />

        <div data-testid="pagination">
          <Pagination
            pageNumber={data?.pageNumber ?? page}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? limit}
            totalCount={data?.totalCount ?? 0}
            hasPreviousPage={data?.hasPreviousPage ?? false}
            hasNextPage={data?.hasNextPage ?? false}
            onPageChange={(p) => updateParams({ page: String(p) })}
            btnPreviousPageTestId="btnPreviousPage"
            btnNextPageTestId="btnNextPage"
            btnCurrentPageTestId="btnCurrentPage"
          />
        </div>
      </div>
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
}

