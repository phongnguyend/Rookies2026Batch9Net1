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
}

export interface AssetListItem {
  id: string;
  assetCode: string;
  name: string;
  category: string;
  state: AssetState;
  location: string;
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
  history: AssetHistoryItem[];
}


// ─── Categories ───────────────────────────────────────────
export interface CategoryItem {
  id: string;
  name: string;
}

export type GetCategoriesResponse = CategoryItem[];
