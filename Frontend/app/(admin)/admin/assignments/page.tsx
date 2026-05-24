"use client";

import { useSearchParams, useRouter, usePathname } from "next/navigation";
import DataTable, {
  SortItem,
  type ColumnDef,
} from "@/features/shared/components/DataTable";
import { useGetAllAssignmentsQuery } from "@/features/assignments/assignments.api";
import { AssignmentDto, AssignmentState } from "@/features/assignments/assignments.types";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import SearchInput from "@/features/shared/components/SearchInput";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import DropdownFilter from "@/features/shared/components/DropdownFilter";

const limit = 10;

export default function AssignmentsPage() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  // Read from Url
  const page = Number(searchParams.get("page")) || 1;
  const search = searchParams.get("search") || "";
  const states = searchParams.getAll("state");
  const assignedDateParam = searchParams.get("assignedDate");
  const assignedDate = assignedDateParam ? new Date(assignedDateParam) : null;
  const sortBy = searchParams.get("sortBy") || undefined;
  const sortDesc = searchParams.get("sortDesc") === "true";

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

  const assignments = data?.items ?? [];

  const columns: ColumnDef<AssignmentDto>[] = [
    {
      key: "no",
      header: "No.",
      render: (_assignment, index) => (page - 1) * limit + index + 1,
    },
    { key: "assetCode", header: "Asset Code", sortable: true },
    { key: "assetName", header: "Asset Name", sortable: true },
    { key: "assignedTo", header: "Assigned To", sortable: true },
    { key: "assignedBy", header: "Assigned By", sortable: true },
    { key: "assignedDate", header: "Assigned Date", sortable: true },
    { key: "state", header: "State", sortable: true },
  ];

  return (
    <div className="min-h-screen bg-white text-[#333]">
      <div className="flex">
        <main className="flex-1 px-32 pt-24">
          <h2 className="mb-6 text-xl font-bold text-primary">
            Assignment List
          </h2>

          <div className="mb-6 flex items-center justify-between">
            {/* Left side */}
            <div className="flex items-center gap-4">
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
              />

              <DatePickerInput
                value={assignedDate}
                onChange={(date) =>
                  updateParams({
                    assignedDate:
                      date?.toLocaleDateString("en-CA"),
                    page: "1",
                  })
                }
                placeholder="Assigned Date"
              />
            </div>

            {/* Right side */}
            <SearchInput
              value={search}
              placeholder="Search..."
              onChange={(value) =>
                updateParams({ search: value })
              }
              onSearch={(value) =>
                updateParams({
                  search: value,
                  page: "1",
                })
              }
            />
          </div>

          <DataTable<AssignmentDto>
            data={assignments}
            columns={columns}
            isLoading={isLoading}
            emptyMessage="No assignments found."
            sorts={sorts}
            onSortChange={(newSorts) =>
              updateParams({
                sortBy: newSorts[0]?.key,
                sortDesc: newSorts[0]?.direction === SortDirection.Desc
                  ? "true"
                  : undefined,
                page: "1",
              })
            }
          />

          <Pagination
            pageNumber={data?.pageNumber ?? page}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? limit}
            totalCount={data?.totalCount ?? 0}
            hasPreviousPage={data?.hasPreviousPage ?? false}
            hasNextPage={data?.hasNextPage ?? false}
            onPageChange={(p) => updateParams({ page: String(p) })}
          />
        </main>
      </div>
    </div>
  );
}
