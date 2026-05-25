import { PaginationResponse } from "@/lib/api/base.types";

export enum UserRoles {
  Admin = "Admin",
  Staff = "Staff"
}

export interface UserRow {
  id: string;
  staffCode: string;
  fullName: string;
  userName: string;
  joinedDate: string;
  userType: UserRoles;
  createdDate: string;
  updatedDate: string;
  canBeDisabled: boolean;
}

export interface GetUsersRequest {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  type?: UserRoles;
  sortBy?: string;
  sortDesc?: boolean;
}

export interface GetUsersResponse extends PaginationResponse<UserRow> {}
