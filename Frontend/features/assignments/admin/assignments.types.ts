import { LookupAssetsSummary } from "@/features/Assets/assets.types";
import { LookupUsers } from "@/features/users/users.types";
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
  isReturning: boolean;
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
  isReturning: boolean;
};

export enum AssignmentState {
  WaitingForAcceptance = "WaitingForAcceptance",
  Accepted = "Accepted",
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

export interface CreateAssignmentRequest {
  userId: string;
  assetId: string;
  assignedDate: string;
  note?: string;
}

export interface AdminCreateReturnRequest {
  assignmentId: string;
}

export interface GetAssignmentsResponse extends PaginationResponse<Assignment> { }

export namespace GetAssignmentForEditing {
  export interface Request {
    assignmentId: string;
  }

  export interface Response {
    id: string;
    user: LookupUsers.LookupUsersSummary;
    asset: LookupAssetsSummary
    assignedDate: string;
    note: string;
  }
}

export namespace AdminEditAssignment {
  export interface Request {
    assignmentId: string;
    payload: AdminEditAssignmentPayload;
  }

  export interface AdminEditAssignmentPayload {
    userId: string;
    assetId: string;
    assignedDate: string;
    note?: string;
  }
}
