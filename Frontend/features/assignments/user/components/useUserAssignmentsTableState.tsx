import { useReducer } from "react";
import { ViewUserAssignments } from "../user-assignment.types";

const DEFAULT_PARAM: ViewUserAssignments.Request = {
  pageNumber: 1,
  pageSize: 10,
};

type Action =
  | { type: "SET_SORTING"; payload: { sortBy: string; sortDesc: boolean } }
  | {
      type: "SET_PAGINATION";
      payload: { pageNumber: number; pageSize: number };
    }
  | { type: "CLEAR_SORTING" };

function reducer(
  state: ViewUserAssignments.Request,
  action: Action,
): ViewUserAssignments.Request {
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
    default:
      return state;
  }
}

export function useUserAssignmentTableState() {
  const [params, dispatch] = useReducer(reducer, DEFAULT_PARAM);
  return { params, dispatch };
}
