"use client";
import {
  useSearchParams,
  useRouter,
  usePathname,
  useParams,
} from "next/navigation";
import { SortItem } from "@/features/shared/components/DataTable";
import { useGetAllAssignmentsQuery } from "@/features/assignments/admin/assignments.api";
import {
  Assignment,
  AssignmentState,
} from "@/features/assignments/admin/assignments.types";
import SearchInput from "@/features/shared/components/SearchInput";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import { useMemo, useState } from "react";
import AssignmentDetailPopup from "../../../../features/assignments/admin/components/AssignmentDetailPopup";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import { displayAssignmentState } from "@/utils/assignment.utils";
import SingleSortDataTable, {
  ColumnDef,
} from "@/features/shared/components/SingleSortDataTable";
import { CircleX, Pencil, RotateCcw } from "lucide-react";
import { useAppSelector } from "@/lib/redux/hooks";
import { selectPromotedAssignment } from "@/features/assignments/admin/edit/admin-assignment-list-ui.selectors";

const limit = 10;

export default function AssignmentsPage() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const promotedAssignment = useAppSelector(selectPromotedAssignment);

  // Read from Url
  const page = Number(searchParams.get("page")) || 1;
  const search = searchParams.get("search") || "";
  const [searchInput, setSearchInput] = useState(search);
  const states = searchParams.getAll("state");
  const assignedDateParam = searchParams.get("assignedDate");
  const assignedDate = assignedDateParam ? new Date(assignedDateParam) : null;
  const sortBy = searchParams.get("sortBy") || undefined;
  const sortDesc = searchParams.get("sortDesc") === "true";
  const [selectedAssignmentId, setSelectedAssignmentId] = useState<
    string | null
  >(null);

  const sorts: SortItem[] = sortBy
    ? [
        {
          key: sortBy,
          direction: sortDesc ? SortDirection.Desc : SortDirection.Asc,
        },
      ]
    : [];

  // Function update URL
  const updateParams = (
    updates: Record<string, string | string[] | undefined>,
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
            assignedDate.getDate(),
          ),
        ).toISOString()
      : undefined,
    sortBy: sortBy,
    sortDirection: sortDesc ? SortDirection.Desc : SortDirection.Asc,
  });

  const displayAssignments = useMemo(() => {
    if (!data?.items) {
      return [];
    }

    if (!promotedAssignment || page !== 1) {
      return data.items;
    }

    return [
      promotedAssignment,
      ...data.items.filter((x) => x.id !== promotedAssignment.id),
    ];
  }, [data?.items, promotedAssignment, page]);

  // const assignments = data?.items ?? [];
  const assignments = displayAssignments;

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
      render: (row) => displayAssignmentState(row.state),
    },
    {
      key: "actions",
      header: "",
      className: "w-28",
      render: (assignment) => {
        const isWaiting = assignment.state === "WaitingForAcceptance";

        const isAccepted = assignment.state === "Accepted";

        const isFinal =
          assignment.state === "Returned" || assignment.state === "Declined";

        return (
          <DataTableButtonActions
            row={assignment}
            disabledAccept={isAccepted || isFinal}
            disabledDecline={isAccepted || isFinal}
            disabledReturn={isWaiting || isFinal}
            onAccept={(row) => {
              router.push(`/admin/assignments/edit?id=${row.id}`);
            }}
            onDecline={(row) => console.log("decline", row)}
            onReturn={(row) => console.log("return", row)}
            acceptBtnTestId="btnAcceptAssignment"
            declineBtnTestId="btnDeclineAssignment"
            returnBtnTestId="btnReturnAssignment"
            acceptIcon={
              <Pencil className="text-gray-500" size={20} strokeWidth={3} />
            }
            declineIcon={<CircleX size={20} strokeWidth={3} />}
            returnIcon={<RotateCcw size={20} strokeWidth={3} />}
          />
        );
      },
    },
  ];

  return (
    <div data-testid="mnuManageAssignment">
      <div className="text-lg font-bold text-primary mb-2">Assignment List</div>

      <div className="mb-4 flex flex-col gap-3 lg:flex-row lg:items-center">
        {/* Left group: State + Assigned Date */}
        <div className="flex flex-wrap gap-3">
          <div data-testid="ddlState" className="w-full sm:w-auto">
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
                          date.getDate(),
                        ),
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
            className="w-full sm:w-auto rounded bg-primary px-5 py-2 font-semibold text-white whitespace-nowrap text-sm sm:text-base"
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
