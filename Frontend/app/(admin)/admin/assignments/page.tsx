"use client";
import { useSearchParams, useRouter, usePathname } from "next/navigation";
import {
  SortItem,
} from "@/features/shared/components/DataTable";
import { useGetAllAssignmentsQuery } from "@/features/assignments/admin/assignments.api";
import { Assignment, AssignmentState } from "@/features/assignments/admin/assignments.types";
import SearchInput from "@/features/shared/components/SearchInput";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import AssignmentTable, { ColumnDef } from "../../../../features/assignments/admin/components/AssignmentTable";
import { useState } from "react";
import AssignmentDetailPopup from "../../../../features/assignments/admin/components/AssignmentDetailPopup";
import AssignmentDateTimePicker from "@/features/assignments/admin/components/AssignmentDateTimePicker";

const limit = 10;

export default function AssignmentsPage() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  // Read from Url
  const page = Number(searchParams.get("page")) || 1;
  const search = searchParams.get("search") || "";
  const [searchInput, setSearchInput] = useState(search);
  const states = searchParams.getAll("state");
  const assignedDateParam = searchParams.get("assignedDate");
  const assignedDate = assignedDateParam ? new Date(assignedDateParam) : null;
  const sortBy = searchParams.get("sortBy") || undefined;
  const sortDesc = searchParams.get("sortDesc") === "true";
  const [selectedAssignmentId, setSelectedAssignmentId] = useState<string | null>(null);

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
  });

  const formatText = (text: string) => {
    return text
      .replace(/([a-z])([A-Z])/g, "$1 $2")
      .replace(/_/g, " ");
  };


  const assignments = data?.items ?? [];

  const columns: ColumnDef<Assignment>[] = [
    {
      key: "no",
      header: "No.",
      className: "w-[40px]",
      render: (_assignment, index) => (page - 1) * limit + index + 1,
    },
    {
      key: "assetCode",
      header: "Asset Code",
      sortable: true,
      TitleColumnTestId: "btnSortAssetCode",
      ColumnTestId: (_row, index) => `colAssetCode-${index}`,
    },
    {
      key: "assetName",
      header: "Asset Name",
      sortable: true,
      TitleColumnTestId: "btnSortAssetName",
      ColumnTestId: (_row, index) => `colAssetName-${index}`,
    },
    {
      key: "assignedTo",
      header: "Assigned to",
      sortable: true,
      TitleColumnTestId: "btnSortAssignedTo",
      ColumnTestId: (_row, index) => `colAssignedTo-${index}`,
    },
    {
      key: "assignedBy",
      header: "Assigned by",
      sortable: true,
      TitleColumnTestId: "btnSortAssignedBy",
      ColumnTestId: (_row, index) => `colAssignedBy-${index}`,
    },
    {
      key: "assignedDate",
      header: "Assigned Date",
      sortable: true,
      TitleColumnTestId: "btnSortAssignedDate",
      ColumnTestId: (_row, index) => `colAssignedDate-${index}`,
    },
    {
      key: "state",
      header: "State",
      sortable: true,
      TitleColumnTestId: "btnSortState",
      ColumnTestId: (_row, index) => `colState-${index}`,
      render: (row) =>
        row.state
          .replace(/([a-z])([A-Z])/g, "$1 $2")
          .toLowerCase()
          .replace(/^./, (char) => char.toUpperCase()),
    },
    {
      key: "actions",
      header: "",
      render: (assignment) => {
        const isWaiting =
          assignment.state === "WaitingForAcceptance";

        const isAccepted =
          assignment.state === "Accepted";

        const isFinal =
          assignment.state === "Returned" ||
          assignment.state === "Declined";

        return (
          <DataTableButtonActions
            row={assignment}
            disabledAccept={isAccepted || isFinal}
            disabledDecline={isAccepted || isFinal}
            disabledReturn={isWaiting || isFinal}
            onAccept={(row) => console.log("accept", row)}
            onDecline={(row) => console.log("decline", row)}
            onReturn={(row) => console.log("return", row)}
            acceptBtnTestId="btnAcceptAssignment"
            declineBtnTestId="btnDeclineAssignment"
            returnBtnTestId="btnReturnAssignment"
          />
        );
      },
    }
  ];

  return (
    <div data-testid="mnuManageAssignment">
      <div className="text-lg font-bold text-primary mb-2">
        Assignment List
      </div>

      <div className="space-y-4">
        <div className="flex items-center justify-between gap-2 flex-wrap sm:flex-nowrap">

          {/* Left group: State + Assigned Date */}
          <div className="flex items-center gap-2 min-w-0">
            <div data-testid="ddlState" className="min-w-0">
              <DropdownFilter
                items={Object.values(AssignmentState).map((s) => ({
                  key: s,
                  label: s,
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

            <div data-testid="dtpAssignedDate" className="min-w-0">
              <AssignmentDateTimePicker
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
                width="w-36 sm:w-50"
              />
            </div>
          </div>

          {/* Right group: Search + Create button */}
          <div className="flex items-center gap-2 min-w-0">
            <div className="min-w-0">
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
              className="rounded bg-primary px-3 sm:px-5 py-2 font-semibold text-white whitespace-nowrap text-sm sm:text-base shrink-0"
              data-testid="btnCreateNewAssignment"
            >
              <span className="sm:hidden">+ New</span>
              <span className="hidden sm:inline">Create new assignment</span>
            </button>
          </div>

        </div>

        <AssignmentTable<Assignment>
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
    </div>
  );
}

