"use client";

import { useState } from "react";
import SingleSortDataTable, {
  ColumnDef,
  SortItem,
} from "@/features/shared/components/SingleSortDataTable";
import Pagination from "@/features/shared/components/Pagination";
import { SortDirection } from "@/lib/api/base.types";
import { useGetReportQuery } from "@/features/report/report.api";
import { ViewReport } from "@/features/report/report.types";

const defaultSort: SortItem = {
  key: "categoryName",
  direction: SortDirection.Asc,
};

// Map frontend column keys to backend SortBy enums
const mapTableKeyToBackendSortBy = (key: string): ViewReport.SortBy => {
  switch (key) {
    case "categoryName":
      return ViewReport.SortBy.Category;
    case "total":
      return ViewReport.SortBy.Total;
    case "assigned":
      return ViewReport.SortBy.Assigned;
    case "available":
      return ViewReport.SortBy.Available;
    case "notAvailable":
      return ViewReport.SortBy.NotAvailable;
    case "waitingForRecycling":
      return ViewReport.SortBy.WaitingForRecycling;
    case "recycled":
      return ViewReport.SortBy.Recycled;
    default:
      return ViewReport.SortBy.Category;
  }
};

export default function ReportPage() {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [pageSize] = useState<number>(10);
  const [sorts, setSorts] = useState<SortItem[]>([defaultSort]);

  const queryParams: ViewReport.Request = {
    pageNumber: pageNumber,
    pageSize,
    sortBy: sorts[0] ? mapTableKeyToBackendSortBy(sorts[0].key) : undefined,
    sortDirection: sorts[0]?.direction ?? undefined,
  };

  const { data, isLoading } = useGetReportQuery(queryParams);
  const reports = data?.items ?? [];

  const handleSortChange = (newSorts: SortItem[]) => {
    setSorts(newSorts);
    setPageNumber(1); // Reset to page 1 on sorting change
  };

  const handleExport = () => {
    console.log("export");
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
              disabled={isLoading || reports.length === 0}
              data-testid="btnExport"
              className="w-full sm:w-auto rounded bg-primary px-5 py-2 font-semibold text-white whitespace-nowrap text-sm sm:text-base disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Export
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
    </div>
  );
}
