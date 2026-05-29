"use client";

import { useCallback, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import DataTable, {
  type ColumnDef,
  type SortItem,
} from "@/features/returns/components/DataTable";
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
import { formatDate } from "@/utils/datetime.utils";

const pageSize = 10;
const returnRequestTimeZone = "Asia/Bangkok";

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

function getTimeZoneOffsetInMilliseconds(date: Date, timeZone: string) {
  const parts = new Intl.DateTimeFormat("en-US", {
    timeZone,
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    hourCycle: "h23",
  }).formatToParts(date);

  const valueByType = new Map(
    parts.map((part) => [part.type, Number(part.value)]),
  );

  return (
    Date.UTC(
      valueByType.get("year") ?? 0,
      (valueByType.get("month") ?? 1) - 1,
      valueByType.get("day") ?? 1,
      valueByType.get("hour") ?? 0,
      valueByType.get("minute") ?? 0,
      valueByType.get("second") ?? 0,
    ) - date.getTime()
  );
}

function getStartOfDayInTimeZoneAsUtc(date: Date, timeZone: string) {
  const utcDate = new Date(
    Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()),
  );

  return new Date(
    utcDate.getTime() - getTimeZoneOffsetInMilliseconds(utcDate, timeZone),
  );
}

function formatReturnedDateForQuery(date: Date | null) {
  if (!date) {
    return undefined;
  }

  return getStartOfDayInTimeZoneAsUtc(
    date,
    returnRequestTimeZone,
  ).toISOString();
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
        data-testid="btnAccept"
      >
        ✓
      </button>
      <button
        type="button"
        disabled={!isWaiting}
        aria-label="Cancelled"
        title="Cancelled"
        className={`text-black ${isWaiting ? enabledClass : disabledClass}`}
        data-testid="btnDecline"
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
      router.replace(queryString ? `?${queryString}` : "/admin/returns", {
        scroll: false,
      });
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
      cellTestId: () => "txtAssetCode",
    },
    {
      key: "assetName",
      header: "Asset Name",
      sortable: true,
      className: "w-[210px]",
      cellTestId: () => "txtAssetName",
    },
    {
      key: "requestedBy",
      header: "Requested by",
      sortable: true,
      className: "w-[126px]",
      cellTestId: () => "txtReported",
    },
    {
      key: "assignedDate",
      header: "Assigned Date",
      sortable: true,
      className: "w-[126px]",
      render: (request) => formatDate(request.assignedDate),
      cellTestId: () => "dpAssignedDate",
    },
    {
      key: "acceptedBy",
      header: "Accepted by",
      sortable: true,
      className: "w-[126px]",
      render: (request) => request.acceptedBy ?? "",
      cellTestId: () => "txtAcceptedBy",
    },
    {
      key: "returnedDate",
      header: "Returned Date",
      sortable: true,
      className: "w-[132px]",
      render: (request) =>
        request.returnedDate ? formatDate(request.returnedDate) : "",
      cellTestId: () => "dpReturnedDate",
    },
    {
      key: "state",
      header: "State",
      sortable: true,
      className: "w-[182px]",
      render: (request) => getStateLabel(request.state),
      cellTestId: () => "txtState",
    },
    {
      key: "actions",
      header: "",
      className: "w-[74px]",
      render: (request) => <RequestActions row={request} />,
    },
  ];

  return (
    <div className="min-h-screen bg-white text-[#333]" data-testid="mnuReturning">
      <div className="flex">
        <main className="flex-1">
          <h2 className="mb-6 text-xl font-bold text-primary">Request List</h2>

          <div className="mb-6 flex items-center justify-between">
            <div className="flex gap-5">
              <div data-testid="ddlState">
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
              </div>

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
                txtInputTestId="dpReturned"
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
              txtInputTestId="txtSearch"
              btnSearchTestId="btnSearch"
            />
          </div>

          <DataTable<ReturnRequestRow>
            data={requests}
            columns={columns}
            isLoading={isLoading}
            emptyMessage="No return requests found."
            sorts={sorts}
            tableTestId="dgdReturningList"
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
