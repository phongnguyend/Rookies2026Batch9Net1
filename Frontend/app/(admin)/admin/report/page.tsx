"use client";

import { useState } from "react";
import SingleSortDataTable, {
  ColumnDef,
  SortItem,
} from "@/features/shared/components/SingleSortDataTable";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import {
  useGetReportQuery,
  useGetExportStatusQuery,
  useStartExportMutation,
  useCancelExportMutation,
} from "@/features/report/report.api";
import {
  ExportReportJobStatus,
  ExportReportSortBy,
  ViewReport,
} from "@/features/report/report.types";
import { ENV_CONFIGS } from "@/lib/config/env";
import ConfirmModal from "@/features/shared/components/Modal/ConfirmModal";
import { useAppDispatch, useAppSelector } from "@/lib/redux/hooks";
import {
  setDownloading,
  setHasNotifiedReady,
} from "@/features/report/report.slice";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";

const defaultSort: SortItem = {
  key: "categoryName",
  direction: SortDirection.Asc,
};

// Map frontend column keys to backend SortBy enums
const mapTableKeyToBackendSortBy = (key: string): ExportReportSortBy => {
  switch (key) {
    case "categoryName":
      return ExportReportSortBy.Category;
    case "total":
      return ExportReportSortBy.Total;
    case "assigned":
      return ExportReportSortBy.Assigned;
    case "available":
      return ExportReportSortBy.Available;
    case "notAvailable":
      return ExportReportSortBy.NotAvailable;
    case "waitingForRecycling":
      return ExportReportSortBy.WaitingForRecycling;
    case "recycled":
      return ExportReportSortBy.Recycled;
    default:
      return ExportReportSortBy.Category;
  }
};

export default function ReportPage() {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [pageSize] = useState<number>(10);
  const [sorts, setSorts] = useState<SortItem[]>([defaultSort]);
  const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
  const { isDownloading } = useAppSelector((state) => state.reportSlice);
  const { isAuthenticated } = useAppSelector((state) => state.authSlice);
  const dispatch = useAppDispatch();

  const queryParams: ViewReport.Request = {
    pageNumber: pageNumber,
    pageSize,
    sortBy: sorts[0] ? mapTableKeyToBackendSortBy(sorts[0].key) : undefined,
    sortDirection: sorts[0]?.direction ?? undefined,
  };

  const { data, isLoading } = useGetReportQuery(queryParams, {
    pollingInterval: 2000, // auto calling request every 2s
    refetchOnFocus: true, // if return to report tab, then calling again
    refetchOnReconnect: true, // if reconnecting after disconnected, then refetch
    skipPollingIfUnfocused: true, // when on other tab, and comeback to Report tab, then polling is continue, otherwise keep polling in the background if set = false
    skip: !isAuthenticated, // if not authenticated, skip long polling the request
  });

  const { data: statusData, refetch: refetchStatus } = useGetExportStatusQuery(
    undefined,
    {
      pollingInterval: 3000,
      refetchOnFocus: true,
      refetchOnReconnect: true,
      skipPollingIfUnfocused: true,
      skip: !isAuthenticated, // if not authenticated, skip long polling the request
    },
  );

  const [startExport, { isLoading: isStartDownloading }] =
    useStartExportMutation();
  const [cancelExport, { isLoading: isCanceling }] = useCancelExportMutation();

  const reports = data?.items ?? [];
  const jobStatus = statusData?.status;
  const downloadUrl = statusData?.downloadUrl;

  const handleSortChange = (newSorts: SortItem[]) => {
    setSorts(newSorts);
    setPageNumber(1); // Reset to page 1 on sorting change
  };

  // State Machine logic for button label and disabled state using RTK Query and data states directly
  const btnText =
    jobStatus === ExportReportJobStatus.Processing
      ? "Generating..."
      : jobStatus === ExportReportJobStatus.ReadyToDownload
        ? "Ready to Download"
        : "Export";

  const isBtnDisabled =
    reports.length === 0 ||
    jobStatus === ExportReportJobStatus.Processing ||
    isStartDownloading ||
    isCanceling ||
    isDownloading;

  const handleExport = async () => {
    if (jobStatus === ExportReportJobStatus.ReadyToDownload) {
      setIsModalOpen(true);
    } else {
      try {
        dispatch(setHasNotifiedReady(false));
        await startExport({
          sortBy: queryParams.sortBy,
          sortDirection: queryParams.sortDirection,
        }).unwrap();
        refetchStatus();
      } catch (err) {
        console.error("Failed to start export:", err);
      }
    }
  };

  const handleDownload = async () => {
    if (downloadUrl) {
      const fullUrl = `${ENV_CONFIGS.apiUrl}${downloadUrl}`;
      try {
        dispatch(setDownloading(true));
        const response = await fetch(fullUrl);
        if (!response.ok) throw new Error("Failed to download file");
        const blob = await response.blob();

        const filename =
          downloadUrl.substring(downloadUrl.lastIndexOf("/") + 1) ||
          "report.xlsx";

        const blobUrl = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = blobUrl;
        link.setAttribute("download", filename);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(blobUrl);

        dispatch(
          enqueueToast({
            message: "Report downloaded successfully",
            type: ToastType.Success,
          }),
        );

        // Remove the file excel after download successfully
        await cancelExport().unwrap();
        refetchStatus(); // refetch the status of the export button when cancel export
      } catch (err) {
        console.error("Failed to download report:", err);
        dispatch(
          enqueueToast({
            message: "Failed to download report",
            type: ToastType.Error,
          }),
        );
      } finally {
        dispatch(setDownloading(false));
        setIsModalOpen(false);
      }
    }
  };

  const handleCancelReport = async () => {
    try {
      await cancelExport().unwrap();
      setIsModalOpen(false);
      refetchStatus();

      // dispatch the toast for cancelling
      dispatch(
        enqueueToast({
          message: "Cancel the download file",
          type: ToastType.Error,
        }),
      );
    } catch (err) {
      console.error("Failed to cancel export:", err);
    }
  };

  const columns: ColumnDef<ViewReport.ReportRow>[] = [
    {
      key: "categoryName",
      header: "Category",
      sortable: true,
      headerTestId: "btnSortCategory",
      cellTestId: (_row, index) => `colCategory-${index}`,
    },
    {
      key: "total",
      header: "Total",
      sortable: true,
      headerTestId: "btnSortTotal",
      cellTestId: (_row, index) => `colTotal-${index}`,
    },
    {
      key: "assigned",
      header: "Assigned",
      sortable: true,
      headerTestId: "btnSortAssigned",
      cellTestId: (_row, index) => `colAssigned-${index}`,
    },
    {
      key: "available",
      header: "Available",
      sortable: true,
      headerTestId: "btnSortAvailable",
      cellTestId: (_row, index) => `colAvailable-${index}`,
    },
    {
      key: "notAvailable",
      header: "Not Available",
      sortable: true,
      headerTestId: "btnSortNotAvailable",
      cellTestId: (_row, index) => `colNotAvailable-${index}`,
    },
    {
      key: "waitingForRecycling",
      header: "Waiting for Recycling",
      sortable: true,
      headerTestId: "btnSortWaitingForRecycling",
      cellTestId: (_row, index) => `colWaitingForRecycling-${index}`,
    },
    {
      key: "recycled",
      header: "Recycled",
      sortable: true,
      headerTestId: "btnSortRecycled",
      cellTestId: (_row, index) => `colRecycled-${index}`,
    },
  ];

  const totalCount = data?.totalCount ?? 0;
  const showPagination = totalCount > pageSize;

  return (
    <div data-testid="tabReport">
      <div className="text-lg font-bold text-primary mb-2">Report</div>

      <div className="space-y-4 mb-8">
        <div className="mb-4 flex flex-col gap-3 lg:flex-row lg:items-center">
          {/* Right-aligned Export Button */}
          <div className="w-full sm:w-auto sm:ml-auto flex flex-col gap-3 sm:flex-row sm:items-center">
            <button
              type="button"
              onClick={handleExport}
              disabled={isBtnDisabled}
              data-testid="btnExport"
              className="w-full sm:w-auto rounded bg-primary px-5 py-2 font-semibold text-white whitespace-nowrap text-sm sm:text-base disabled:opacity-50 disabled:cursor-not-allowed hover:opacity-90 active:scale-[0.97] transition-all duration-150"
            >
              {btnText}
            </button>
          </div>
        </div>

        <div className="overflow-x-auto" data-testid="dgdReportList">
          <SingleSortDataTable<ViewReport.ReportRow>
            data={reports}
            columns={columns}
            isLoading={isLoading}
            emptyMessage="No reports found."
            sorts={sorts}
            onSortChange={handleSortChange}
          />
        </div>

        {showPagination && (
          <div data-testid="pagination">
            <Pagination
              pageNumber={pageNumber}
              totalPages={data?.totalPages ?? 1}
              pageSize={pageSize}
              totalCount={totalCount}
              hasPreviousPage={pageNumber > 1}
              hasNextPage={pageNumber < (data?.totalPages ?? 1)}
              onPageChange={(nextPage) => setPageNumber(nextPage)}
              btnPreviousPageTestId="btnPrevPage"
              btnNextPageTestId="btnNextPage"
            />
          </div>
        )}
      </div>

      <ConfirmModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onYes={handleDownload}
        onNo={handleCancelReport}
        title="Your report is ready for downloading"
        body="Would you like to download the report or cancel it?"
        yesButtonLabel="Download File"
        noButtonLabel="Cancel Report"
        isLoading={isCanceling || isDownloading}
      />
    </div>
  );
}
