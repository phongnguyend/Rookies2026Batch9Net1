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