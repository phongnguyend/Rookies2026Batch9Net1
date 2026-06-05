import type { PaginationRequest, PaginationResponse } from "@/lib/api/base.types";

export enum AssetState {
  Available = "Available",
  NotAvailable = "NotAvailable",
  Assigned = "Assigned",
  WaitingForRecycling = "WaitingForRecycling",
  Recycled = "Recycled",
}

// ─── List ─────────────────────────────────────────────
export interface GetAssetsRequest extends PaginationRequest {
  categories?: string[];
  states?: AssetState[];
  search?: string;
  sortBy?: string;
  sortDirection?: string;
  isCreatedNewAsset?: boolean
}

export interface AssetListItem {
  id: string;
  assetCode: string;
  name: string;
  category: string;
  state: AssetState;
  location: string;
  hasHistory: boolean;
}

export type GetAssetsResponse = PaginationResponse<AssetListItem>;

// ─── Detail ───────────────────────────────────────────
export interface AssetHistoryItem {
  assignedAtUtc: string;
  assignedTo: string;
  assignedBy: string;
  returnedAtUtc?: string | null;
}

export interface GetAssetDetailResponse {
  id: string;
  assetCode: string;
  name: string;
  specification: string;
  installedAtUtc: string;
  state: AssetState;
  category: string;
  location: string;
  isdeleted: boolean;
  history: AssetHistoryItem[];
}

// ─── Categories ───────────────────────────────────────────
export interface CategoryItem {
  id: string;
  name: string;
  prefix: string;
}
export interface CreateCategoryRequest {
  categoryName: string;
  categoryPrefix: string;
}
export interface CreateCategoryResponse {
  id: string;
  name: string;
  prefix: string;
}

export type GetCategoriesResponse = CategoryItem[];

// ─── Create Assets ───────────────────────────────────────────
export interface CreateAssetRequest {
  assetName: string;
  specification: string;
  installedDate: string;
  state: string;
  categoryId: string;
}

export interface CreateAssetResponse {
  id: string;
  assetCode: string;
  assetName: string;
  category: string;
  state: AssetState;
  location: string;
}

// ─── Look up Asset ───────────────────────────────────────────
export interface LookupAssetsRequest{
  searchTerm?: string;
  sortBy?: string;
  sortDesc?: boolean;
  pageSize?: number;
  pageNumber?: number;
}

export interface LookupAssetsWithAssignedRequest{
  searchTerm?: string;
  sortBy?: string;
  sortDesc?: boolean;
  pageSize?: number;
  pageNumber?: number;
  assignedAssetId?: string;
}

export interface LookupAssetsSummary{
  id: string;
  assetCode: string;
  assetName: string;
  category: string;
}

export interface LookupAssetsResponse extends PaginationResponse<LookupAssetsSummary> {}

// ─── Edit Assets ───────────────────────────────────────────
export interface EditAssetRequest {
  assetId: string;
  assetName: string;
  specification: string;
  installedDate: string;
  state: string;
}

export interface EditAssetResponse {
  id: string;
  assetCode: string;
  assetName: string;
  specification: string;
  installedAtUtc: string;
  state: string;
  category: string;
  location: string;
  hasHistory: boolean;
}
// ─── Delete Assets ───────────────────────────────────────────
export interface DeleteAssetResponse {
  id: string;
  assetCode: string;
  assetName: string;
  isDeleted: boolean;
  deletedTime?: string | null;
}
