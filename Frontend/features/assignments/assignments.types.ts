import {
  PaginationRequest,
  PaginationResponse,
  SortDirection,
} from "@/lib/api/base.types";

export interface Assignment {
  id: string;
  assignedAtUtc: string;
  note?: string;
  state: AssignmentState;
  isReturning: boolean;
  assetId: string;
  assignedByUserId: string;
  assignedToUserId: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
  isDeleted: boolean;
  deletedAtUtc?: string | null;
}

export interface AssignmentDto {
  id: string;
  assetCode: string;
  assetName: string;
  assignedTo: string;
  assignedBy: string;
  assignedDate: string;
  state: string;
}

export enum AssignmentState {
  WaitingForAcceptance = "WaitingForAcceptance",
  Accepted = "Accepted",
  Declined = "Declined",
  Returned = "Returned",
}

export interface GetAssignmentsRequest
  extends PaginationRequest {
  searchTerm?: string;
  state?: string[];
  sortBy?: string;
  sortDirection?: SortDirection;
  assignedDate?: string;
  includeDeleted?: boolean;
}

export interface GetAssignmentsResponse extends PaginationResponse<AssignmentDto> { }
