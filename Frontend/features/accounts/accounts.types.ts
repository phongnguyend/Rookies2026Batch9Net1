import { PaginationResponse } from "@/lib/api/base.types";

export enum AccountRole {
  Admin = "Admin",
  Staff = "Staff",
}

export interface UserRow {
  id: string;
  staffCode: string;
  fullName: string;
  userName: string;
  joinedDate: string;
  userType: AccountRole;
  createdDate: string;
  updatedDate: string;
  canBeDisabled: boolean;
}

export interface GetUsersRequest {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  type?: AccountRole;
  sortBy?: string;
  sortDesc?: boolean;
}

export interface GetUsersResponse extends PaginationResponse<UserRow> {}
