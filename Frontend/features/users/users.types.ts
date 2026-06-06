import { PaginationResponse } from "@/lib/api/base.types";

export enum UserRoles {
  Admin = "Admin",
  Staff = "Staff"
}

export enum Gender {
  Male = "Male",
  Female = "Female",
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
  concurrencyStamp: string;
}

export interface EditUserRequest {
  userId: string;
  dateOfBirth: string;
  gender: Gender;
  joinedDate: string;
  type: UserRoles;
  concurrencyStamp: string;
}

export interface EditUserResponse {
  id: string;
}
export namespace LookupUsers{
  export interface Request {
    searchTerm?: string;
    sortBy?: string;
    sortDesc?: boolean;
    pageSize?: number;
    pageNumber?: number;
  }

  export interface LookupUsersSummary {
    id: string;
    staffCode: string;
    fullName: string;
    type: string;
  }

  export interface Response extends PaginationResponse<LookupUsersSummary> {}
}


export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  dayOfBirth: string;
  joinedDate: string;
  gender: Gender;
  userType: UserRoles;
}

export interface CreateUserResponse {
  id: string;
  staffCode: string;
  userName: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string | null;
  joinedDate: string;
  userType: UserRoles;
  gender: Gender;
}
