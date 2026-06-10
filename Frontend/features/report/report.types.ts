import {
  PaginationRequest,
  PaginationResponse,
  SortDirection,
} from "@/lib/api/base.types";

export enum ExportReportJobStatus {
  Failed = "Failed",
  Processing = "Processing",
  ReadyToDownload = "ReadyToDownload",
}

export enum ExportReportSortBy {
  Category = "Category",
  Total = "Total",
  Assigned = "Assigned",
  Available = "Available",
  NotAvailable = "NotAvailable",
  WaitingForRecycling = "WaitingForRecycling",
  Recycled = "Recycled",
}

export namespace ViewReport {

  export interface ReportRow {
    categoryId: string;
    categoryName: string;
    total: number;
    assigned: number;
    available: number;
    notAvailable: number;
    waitingForRecycling: number;
    recycled: number;
  }

  export interface Request extends PaginationRequest {
    sortBy?: ExportReportSortBy;
    sortDirection?: SortDirection;
  }

  export interface Response extends PaginationResponse<ReportRow> { }
}

export namespace Export {
  export interface Request {
    sortBy?: ExportReportSortBy;
    sortDirection?: SortDirection;
  }
  export interface Response {
    status: ExportReportJobStatus;
  }
}

export namespace CurrentDownload {
  export interface Request { }
  export interface Response {
    status: ExportReportJobStatus | null;
    downloadUrl: string | null;
    completedAtUtc: string | null;
  }
}

export namespace Cancel {
  export interface Request { }
  export interface Response {
    success: boolean;
  }
}
