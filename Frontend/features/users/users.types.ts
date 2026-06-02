import { PaginationResponse } from "@/lib/api/base.types";

export enum UserRoles {
  Admin = "Admin",
  Staff = "Staff"
}

export enum Gender {
  Male = "Male",
  Female = "Female"
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

export interface UserDetail {
  id: string;
  staffCode: string;
  fullName: string;
  userName: string;
  joinedDate: string;
  userType: UserRoles;
  dateOfBirth: string;
  gender: string;
  location: string;
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

export type GetUserByIdResponse = UserDetail;

export interface GetUserForEditResponse {
  id: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string | null;
  gender: Gender;
  joinedDate: string;
  userType: UserRoles;
  isCurrentUser: boolean;
}

export interface EditUserRequest {
  userId: string;
  dateOfBirth: string;
  gender: Gender;
  joinedDate: string;
  type: UserRoles;
}

export interface EditUserResponse {
  id: string;
}
