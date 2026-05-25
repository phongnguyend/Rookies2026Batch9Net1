import {
  PaginationRequest,
  PaginationResponse,
  SortDirection,
} from "@/lib/api/base.types";

export interface Assignment {
  id: string;
  assetCode: string;
  assetName: string;
  assignedTo: string;
  assignedBy: string;
  assignedDate: string;
  state: string;
}

export type AssignmentDetails = {
  assetCode: string;
  assetName: string;
  specification: string;
  assignedTo: string;
  assignedBy: string;
  assignedDate: string;
  state: string;
  note: string;
};

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

export interface GetAssignmentsResponse extends PaginationResponse<Assignment> { }
