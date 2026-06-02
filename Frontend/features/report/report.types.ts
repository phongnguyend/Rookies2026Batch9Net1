import {
  PaginationRequest,
  PaginationResponse,
  SortDirection,
} from "@/lib/api/base.types";



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

  export enum SortBy {
    Category = "Category",
    Total = "Total",
    Assigned = "Assigned",
    Available = "Available",
    NotAvailable = "NotAvailable",
    WaitingForRecycling = "WaitingForRecycling",
    Recycled = "Recycled",
  }

  export interface Request extends PaginationRequest {
    sortBy?: SortBy;
    sortDirection?: SortDirection;
  }

  export interface Response extends PaginationResponse<ReportRow> { }
}
