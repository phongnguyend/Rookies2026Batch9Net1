export interface PaginationRequest {
    pageIndex?: number;
    pageSize?: number;
}

export interface PaginationResponse<T> {
    items: T[];
    totalItems: number;
    totalPages: number;
    pageIndex: number;
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