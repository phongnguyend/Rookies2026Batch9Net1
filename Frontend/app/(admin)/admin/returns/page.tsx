"use client";

import { useCallback, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import DataTable, {
  type ColumnDef,
  type SortItem,
} from "@/features/shared/components/DataTable";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import DatePickerInput from "@/features/returns/components/DatePickerInput";
import { useGetReturnRequestsQuery } from "@/features/returns/returns.api";
import {
  ReturnRequestState,
  type ReturnRequestRow,
} from "@/features/returns/returns.types";
import { SortDirection } from "@/lib/api/base.types";

const pageSize = 10;

const stateFilters = [
  { id: ReturnRequestState.Completed, label: "Completed" },
  { id: ReturnRequestState.WaitingForReturning, label: "Waiting for returning" },
  { id: ReturnRequestState.Cancelled, label: "Cancelled" }
];

function parseDateParam(value: string | null) {
  if (!value) {
    return null;
  }

  if (value.includes("T")) {
    const date = new Date(value);

    return Number.isNaN(date.getTime()) ? null : date;
  }

  const [year, month, day] = value.split("-").map(Number);

  if (!year || !month || !day) {
    return null;
  }

  return new Date(year, month - 1, day);
}

function formatReturnedDateForQuery(date: Date | null) {
  if (!date) {
    return undefined;
  }

  // The picker gives a local calendar day. The API stores UTC, so send the
  // start of that local day converted back to UTC by subtracting 7 hours.
  const utcDate = new Date(
    Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()) -
      7 * 60 * 60 * 1000,
  );

  return utcDate.toISOString();
}

function formatUtcDateToUtcPlus7(utc?: string | null) {
  if (!utc) {
    return "";
  }

  const hasTimeZone = /(?:z|[+-]\d{2}:?\d{2})$/i.test(utc.trim());
  const date = new Date(hasTimeZone ? utc : `${utc}Z`);

  if (Number.isNaN(date.getTime())) {
    return "";
  }

  const parts = new Intl.DateTimeFormat("en-CA", {
    timeZone: "Asia/Bangkok",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(date);

  const year = parts.find((part) => part.type === "year")?.value;
  const month = parts.find((part) => part.type === "month")?.value;
  const day = parts.find((part) => part.type === "day")?.value;

  return year && month && day ? `${year}-${month}-${day}` : "";
}

function getStateLabel(state: string) {
  if (state === ReturnRequestState.WaitingForReturning) {
    return "Waiting for returning";
  }

  return state;
}

function RequestActions({ row }: { row: ReturnRequestRow }) {
  const isWaiting = row.state === ReturnRequestState.WaitingForReturning;
  const disabledClass = "cursor-not-allowed opacity-35";
  const enabledClass =
    "cursor-pointer transition hover:scale-125 hover:text-primary";

  return (
    <div className="flex items-center justify-center gap-4 text-xl font-bold leading-none">
      <button
        type="button"
        disabled={!isWaiting}
        aria-label="Completed"
        title="Completed"
        className={`text-primary ${isWaiting ? enabledClass : disabledClass}`}
      >
        ✓
      </button>
      <button
        type="button"
        disabled={!isWaiting}
        aria-label="Cancelled"
        title="Cancelled"
        className={`text-black ${isWaiting ? enabledClass : disabledClass}`}
      >
        ×
      </button>
    </div>
  );
}

export default function ReturnsPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const pageParam = Number(searchParams.get("page") ?? "1");
  const page = Number.isNaN(pageParam) || pageParam < 1 ? 1 : pageParam;
  const querySearch = searchParams.get("search") ?? "";
  const statesParam = searchParams.get("states");
  const selectedStates = (statesParam?.split(",") ?? []).filter((state) =>
    stateFilters.some((item) => item.id === state),
  );
  const returnedDate = parseDateParam(searchParams.get("returnedDate"));
  const sortByParam = searchParams.get("sortBy") ?? "";
  const sortDescParam = searchParams.get("sortDesc");
  const sorts: SortItem[] = sortByParam
    ? [
        {
          key: sortByParam,
          direction:
            sortDescParam === "true" ? SortDirection.Desc : SortDirection.Asc,
        },
      ]
    : [];

  const [searchState, setSearchState] = useState({
    inputValue: querySearch,
    urlValue: querySearch,
  });
  const searchInput =
    searchState.urlValue === querySearch ? searchState.inputValue : querySearch;

  const updateQueryParams = useCallback(
    (
      params: Partial<{
        page: number;
        search: string;
        states: string[] | null;
        returnedDate: string | null;
        sortBy: string | null;
        sortDesc: boolean | null;
      }>,
    ) => {
      const nextSearchParams = new URLSearchParams(searchParams.toString());

      if (params.page !== undefined) {
        if (params.page > 1) {
          nextSearchParams.set("page", String(params.page));
        } else {
          nextSearchParams.delete("page");
        }
      }

      if (params.search !== undefined) {
        const value = params.search.trim();
        if (value) {
          nextSearchParams.set("search", value);
        } else {
          nextSearchParams.delete("search");
        }
      }

      if (params.states !== undefined) {
        if (params.states && params.states.length > 0) {
          nextSearchParams.set("states", params.states.join(","));
        } else {
          nextSearchParams.delete("states");
        }
      }

      if (params.returnedDate !== undefined) {
        if (params.returnedDate) {
          nextSearchParams.set("returnedDate", params.returnedDate);
        } else {
          nextSearchParams.delete("returnedDate");
        }
      }

      if (params.sortBy !== undefined) {
        if (params.sortBy) {
          nextSearchParams.set("sortBy", params.sortBy);
        } else {
          nextSearchParams.delete("sortBy");
        }
      }

      if (params.sortDesc !== undefined) {
        if (params.sortDesc) {
          nextSearchParams.set("sortDesc", String(params.sortDesc));
        } else {
          nextSearchParams.delete("sortDesc");
        }
      }

      const queryString = nextSearchParams.toString();
      router.replace(queryString ? `?${queryString}` : "/admin/returns");
    },
    [router, searchParams],
  );

  const { data, isLoading } = useGetReturnRequestsQuery({
    pageNumber: page,
    pageSize,
    ...(querySearch ? { searchTerm: querySearch } : {}),
    ...(selectedStates.length > 0 ? { states: selectedStates } : {}),
    ...(returnedDate
      ? { returnedDate: formatReturnedDateForQuery(returnedDate) }
      : {}),
    ...(sorts[0]?.key ? { sortBy: sorts[0].key } : {}),
    ...(sorts[0]?.direction ? { sortDirection: sorts[0].direction } : {}),
  });

  const requests = data?.items ?? [];

  const columns: ColumnDef<ReturnRequestRow>[] = [
    {
      key: "no",
      header: "No.",
      className: "w-[58px]",
      render: (_request, index) => (page - 1) * pageSize + index + 1,
    },
    {
      key: "assetCode",
      header: "Asset Code",
      sortable: true,
      className: "w-[110px]",
    },
    {
      key: "assetName",
      header: "Asset Name",
      sortable: true,
      className: "w-[210px]",
    },
    {
      key: "requestedBy",
      header: "Requested by",
      sortable: true,
      className: "w-[126px]",
    },
    {
      key: "assignedDate",
      header: "Assigned Date",
      sortable: true,
      className: "w-[126px]",
      render: (request) => formatUtcDateToUtcPlus7(request.assignedDate),
    },
    {
      key: "acceptedBy",
      header: "Accepted by",
      sortable: true,
      className: "w-[126px]",
      render: (request) => request.acceptedBy ?? "",
    },
    {
      key: "returnedDate",
      header: "Returned Date",
      sortable: true,
      className: "w-[132px]",
      render: (request) => formatUtcDateToUtcPlus7(request.returnedDate),
    },
    {
      key: "state",
      header: "State",
      sortable: true,
      className: "w-[182px]",
      render: (request) => getStateLabel(request.state),
    },
    {
      key: "actions",
      header: "",
      className: "w-[74px]",
      render: (request) => <RequestActions row={request} />,
    },
  ];

  return (
    <div className="min-h-screen bg-white text-[#333]">
      <div className="flex">
        <main className="flex-1 px-6 pt-4">
          <h2 className="mb-6 text-lg font-bold text-primary">Request List</h2>

          <div className="mb-6 flex items-center justify-between">
            <div className="flex gap-5">
              <DropdownFilter
                items={stateFilters}
                values={selectedStates}
                placeholder="State"
                width="w-[200px]"
                getKey={(state) => state.id}
                getLabel={(state) => state.label}
                onChange={(values) => {
                  // Filter changes always restart the list from page 1.
                  updateQueryParams({
                    page: 1,
                    states:
                      values.length === stateFilters.length ? [] : values,
                  });
                }}
                allLabel="All"
              />

              <DatePickerInput
                value={returnedDate}
                onChange={(date) => {
                  // Filter changes always restart the list from page 1.
                  updateQueryParams({
                    page: 1,
                    returnedDate: date
                      ? formatReturnedDateForQuery(date)!
                      : null,
                  });
                }}
                placeholder="Returned Date"
                width="w-[220px]"
              />
            </div>

            <SearchInput
              value={searchInput}
              placeholder=""
              width="w-[242px]"
              onChange={(value) =>
                setSearchState({ inputValue: value, urlValue: querySearch })
              }
              onSearch={(value) => {
                const nextSearch = value.trim();
                setSearchState({
                  inputValue: nextSearch,
                  urlValue: nextSearch,
                });
                // Search changes always restart the list from page 1.
                updateQueryParams({ page: 1, search: nextSearch });
              }}
            />
          </div>

          <DataTable<ReturnRequestRow>
            data={requests}
            columns={columns}
            isLoading={isLoading}
            emptyMessage="No return requests found."
            sorts={sorts}
            onSortChange={(newSorts) => {
              const nextSort = newSorts.at(-1);
              updateQueryParams({
                page: 1,
                sortBy: nextSort?.key ?? null,
                sortDesc:
                  nextSort?.direction === SortDirection.Desc ? true : null,
              });
            }}
          />

          <Pagination
            pageNumber={data?.pageNumber ?? page}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? pageSize}
            totalCount={data?.totalCount ?? 0}
            hasPreviousPage={data?.hasPreviousPage ?? false}
            hasNextPage={data?.hasNextPage ?? false}
            onPageChange={(nextPage) => updateQueryParams({ page: nextPage })}
          />
        </main>
      </div>
    </div>
  );
}
