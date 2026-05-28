export interface PaginationRequest {
  pageNumber?: number;
  pageSize?: number;
}

export interface PaginationResponse<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  pageSize: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ValidationErrorField {
  field: string;
  messages: string;
}

export interface ApiErrorResponse {
  title: string;
  type: string;
  status: number;
  detail: string;
  errors?: ValidationErrorField[];
}

export enum SortDirection {
  Asc = "Asc",
  Desc = "Desc",
}

export enum AssignmentState {
  WaitingForAcceptance = "WaitingForAcceptance",
  Accepted = "Accepted",
  Declined = "Declined",
  Returned = "Returned",
}

// Enums for Date Picker
export enum Month {
  January = "January",
  February = "February",
  March = "March",
  April = "April",
  May = "May",
  June = "June",
  July = "July",
  August = "August",
  September = "September",
  October = "October",
  November = "November",
  December = "December",
}
