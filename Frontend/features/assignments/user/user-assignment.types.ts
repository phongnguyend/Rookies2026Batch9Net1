import { PaginationResponse } from "@/lib/api/base.types";

export namespace ViewUserAssignments {
  export interface Request {
    sortBy?: string;
    sortDesc?: boolean;
    pageSize?: number;
    pageNumber?: number;
  }

  export interface UserAssignmentSummary {
    id: string;
    assetCode: string;
    assetName: string;
    category: string;
    assignedDate: string;
    state: string;
    isReturning: boolean;
  }

  export interface Response extends PaginationResponse<UserAssignmentSummary> {}
}

export namespace ViewUserAssignmentDetail {
  export interface Request {
    assignmentId: string;
  }

  export interface Response {
    assignmentId: string;
    assetCode: string;
    assetName: string;
    specification: string;
    assignerName: string;
    assigneeName: string;
    assignedDate: string;
    state: string;
    note?: string;
  }
}
