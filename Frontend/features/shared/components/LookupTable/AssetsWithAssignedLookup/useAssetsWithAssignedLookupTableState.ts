import { LookupAssetsWithAssignedRequest } from "@/features/Assets/assets.types";
import { useReducer } from "react";

const DEFAULT_PARAM: LookupAssetsWithAssignedRequest = {
  pageNumber: 1,
  pageSize: 10,
};

type Action =
  | { type: "SET_SORTING"; payload: { sortBy: string; sortDesc: boolean } }
  | {
      type: "SET_PAGINATION";
      payload: { pageNumber: number; pageSize: number };
    }
  | { type: "CLEAR_SORTING" }
  | { type: "SET_SEARCH"; payload: { searchTerm: string } };

function reducer(
  state: LookupAssetsWithAssignedRequest,
  action: Action,
): LookupAssetsWithAssignedRequest {
  switch (action.type) {
    case "SET_SORTING":
      return {
        ...state,
        sortBy: action.payload.sortBy,
        sortDesc: action.payload.sortDesc,
        pageNumber: 1,
      };
    case "SET_PAGINATION":
      return {
        ...state,
        pageNumber: action.payload.pageNumber,
        pageSize: action.payload.pageSize,
      };
    case "CLEAR_SORTING":
      return {
        ...state,
        sortBy: undefined,
        sortDesc: undefined,
        pageNumber: 1,
      };
    case "SET_SEARCH":
      return {
        ...state,
        searchTerm: action.payload.searchTerm,
        pageNumber: 1,
      };
    default:
      return state;
  }
}

export function useAssetsWithAssignedLookupTableState(
  assignedAssetId?: string,
) {
  const [params, dispatch] = useReducer(reducer, {
    ...DEFAULT_PARAM,
    assignedAssetId,
  });
  return { params, dispatch };
}
