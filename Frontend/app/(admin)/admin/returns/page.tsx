"use client";

import { useCallback, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import DataTableButtonActions from "@/features/shared/components/DataTableButtonActions";
import SingleSortDataTable, {
  type ColumnDef,
  type SortItem,
} from "@/features/shared/components/SingleSortDataTable";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import {
  returnsApi,
  useGetReturnRequestsQuery,
} from "@/features/returns/returns.api";
import {
  ReturnRequestState,
  type ReturnRequestRow,
} from "@/features/returns/returns.types";
import { SortDirection } from "@/lib/api/base.types";
import { formatDate } from "@/utils/datetime.utils";
import { Check, X } from "lucide-react";
import ConfirmModal from "@/features/shared/components/Modal/ConfirmModal";
import { useDispatch } from "react-redux";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import DatePickerInput from "@/features/shared/components/DatePickerInput";
import SingleSelectDropdown from "@/features/shared/components/SingleSelectDropdown";

const pageSize = 10;
const returnRequestTimeZone = "Asia/Bangkok";
const defaultSort: SortItem = {
  key: "assetCode",
  direction: SortDirection.Asc,
};

const stateFilters = [
  { id: ReturnRequestState.Completed, label: "Completed" },
  {
    id: ReturnRequestState.WaitingForReturning,
    label: "Waiting for returning",
  },
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

function RequestActions({
  row,
  onCompleteClick,
  onCancelClick,
}: {
  row: ReturnRequestRow;
  onCompleteClick: (row: ReturnRequestRow) => void;
  onCancelClick: (row: ReturnRequestRow) => void;
}) {
  const isWaiting = row.state === ReturnRequestState.WaitingForReturning;

  return (
    <DataTableButtonActions
      row={row}
      disabledAccept={!isWaiting}
      disabledDecline={!isWaiting}
      onAccept={() => onCompleteClick(row)}
      onDecline={() => onCancelClick(row)}
      acceptBtnTestId="btnAccept"
      declineBtnTestId="btnDecline"
      acceptIcon={<Check className="text-primary" strokeWidth={3} size={20} />}
      declineIcon={<X className="text-black" strokeWidth={3} size={20} />}
    />
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
    : [defaultSort];

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

  const { data, isLoading, refetch } = useGetReturnRequestsQuery({
    pageNumber: page,
    pageSize,
    ...(querySearch ? { searchTerm: querySearch } : {}),
    ...(selectedStates.length > 0 ? { states: selectedStates } : {}),
    ...(returnedDate
      ? { returnedDate: formatReturnedDateForQuery(returnedDate) }
      : {}),
    sortBy: sorts[0]?.key ?? defaultSort.key,
    sortDirection: sorts[0]?.direction ?? defaultSort.direction,
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
      render: (request) => (
        <RequestActions
          row={request}
          onCompleteClick={setCompletingRequest}
          onCancelClick={setCancelingRequest}
        />
      ),
    },
  ];

  const dispatchAction = useDispatch();

  const [completeReturnRequest, { isLoading: isCompleting }] =
    returnsApi.useCompleteReturnRequestMutation();

  const [completingRequest, setCompletingRequest] =
    useState<ReturnRequestRow | null>(null);

  const [cancelingRequest, setCancelingRequest] =
    useState<ReturnRequestRow | null>(null);

  const [cancelReturnRequest, { isLoading: isCanceling }] =
    returnsApi.useCancelReturnRequestMutation();

  const handleConfirmComplete = async () => {
    if (!completingRequest) return;

    try {
      await completeReturnRequest({
        returnRequestId: completingRequest.id,
      }).unwrap();

      setCompletingRequest(null);

      dispatchAction(
        enqueueToast({
          message: "Returning request completed successfully.",
          type: ToastType.Success,
          testId: "toastSuccess",
        }),
      );
    } catch (error) {
      setCompletingRequest(null);

      dispatchAction(
        enqueueToast({
          message: "Failed to complete returning request. Please try again.",
          type: ToastType.Error,
          testId: "toastError",
        }),
      );
    }
  };

  const handleConfirmCancel = async () => {
    if (!cancelingRequest) return;
    try {
      await cancelReturnRequest({
        returnRequestId: cancelingRequest.id,
      }).unwrap();
      setCancelingRequest(null);

      dispatchAction(
        enqueueToast({
          message: "Returning request cancelled successfully.",
          type: ToastType.Success,
          testId: "toastSuccess",
        }),
      );
    } catch (error) {
      setCancelingRequest(null);
      dispatchAction(
        enqueueToast({
          message: "Failed to cancel returning request. Please try again.",
          type: ToastType.Error,
          testId: "toastError",
        }),
      );
    }
  };

  return (
    <div
      className="mb-10"
      data-testid="mnuReturning"
    >
      <div className="flex min-w-0">
        <main className="min-w-0 flex-1">
          <h2 className="mb-6 text-xl font-bold text-primary">Request List</h2>

          <div className="my-4 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
            <div className="flex flex-col gap-4 sm:flex-row sm:gap-5 sm:items-center">
              <div data-testid="ddlState">
                <SingleSelectDropdown
                  items={stateFilters}
                  value={selectedStates[0]}
                  placeholder="State"
                  width="w-full sm:w-[200px]"
                  getKey={(state) => state.id}
                  getLabel={(state) => state.label}
                  onChange={(value) => {
                    // Filter changes always restart the list from page 1.
                    updateQueryParams({
                      page: 1,
                      states: value ? [value] : [],
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
                width="w-full sm:w-[220px]"
                txtInputTestId="dpReturned"/>
            </div>

            <div className="flex flex-col gap-4 sm:flex-row sm:items-center lg:gap-8">
              <SearchInput
                value={searchInput}
                placeholder="Search..."
                width="w-full sm:w-[242px]"
                onChange={(value) =>
                  setSearchState({ inputValue: value, urlValue: querySearch })
                }
                onSearch={(value) => {
                  const nextSearch = value.trim();
                  setSearchState({
                    inputValue: nextSearch,
                    urlValue: nextSearch,
                  });

                  if (nextSearch === querySearch && page === 1) {
                    refetch();
                    return;
                  }

                  // Search changes always restart the list from page 1.
                  updateQueryParams({ page: 1, search: nextSearch });
                }}
                txtInputTestId="txtSearch"
                btnSearchTestId="btnSearch"
              />
            </div>
          </div>

          <div className="relative">
            <SingleSortDataTable<ReturnRequestRow>
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
          </div>

          <Pagination
            pageNumber={data?.pageNumber ?? page}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? pageSize}
            totalCount={data?.totalCount ?? 0}
            hasPreviousPage={data?.hasPreviousPage ?? false}
            hasNextPage={data?.hasNextPage ?? false}
            onPageChange={(nextPage) => updateQueryParams({ page: nextPage })}
          />

          {/* Complete Return Request Modal */}
          <ConfirmModal
            isOpen={!!completingRequest}
            onClose={() => setCompletingRequest(null)}
            onYes={handleConfirmComplete}
            isLoading={isCompleting}
            title="Are you sure?"
            body={
              <p data-testid="txtCompleteRequestMessage">
                Do you want to mark this returning request as completed?
              </p>
            }
            yesButtonLabel="Yes"
            noButtonLabel="No"
            modalTestId="CompleteReturningRequestModal"
            confirmBtnTestId="btnConfirmCompleteRequest"
            cancelBtnTestId="btnCancelCompleteRequest"
          />

          {/* Cancel Return Request Modal */}
          <ConfirmModal
            isOpen={!!cancelingRequest}
            onClose={() => setCancelingRequest(null)}
            onYes={handleConfirmCancel}
            isLoading={isCanceling}
            title="Are you sure?"
            body={
              <p data-testid="txtCancelRequestMessage">
                Do you want to cancel this returning request?
              </p>
            }
            yesButtonLabel="Yes"
            noButtonLabel="No"
            modalTestId="CancelReturningRequestModal"
            confirmBtnTestId="btnConfirmCancelRequest"
            cancelBtnTestId="btnCancelCancelRequest"
          />
        </main>
      </div>
    </div>
  );
}
