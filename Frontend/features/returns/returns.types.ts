import {
  PaginationRequest,
  PaginationResponse,
  SortDirection,
} from "@/lib/api/base.types";

export enum ReturnRequestState {
  WaitingForReturning = "WaitingForReturning",
  Completed = "Completed",
  Cancelled = "Cancelled",
}

export interface ReturnRequestRow {
  id: string;
  assetCode: string;
  assetName: string;
  requestedBy: string;
  assignedDate: string;
  acceptedBy?: string;
  returnedDate?: string;
  state: ReturnRequestState;
}

export interface GetReturnRequestsRequest extends PaginationRequest {
  searchTerm?: string;
  states?: string[];
  returnedDate?: string;
  sortBy?: string;
  sortDirection?: SortDirection;
}

export interface GetReturnRequestsResponse
  extends PaginationResponse<ReturnRequestRow> {}

export interface CompleteReturnRequestRequest {
  returnRequestId: string;
}

export interface CancelReturnRequestRequest {
  returnRequestId: string;
}
