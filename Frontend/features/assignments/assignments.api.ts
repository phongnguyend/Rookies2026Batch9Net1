import { SortDirection } from "@/lib/api/base.types";
import {
  AssignmentDetails,
  GetAssignmentsRequest,
  GetAssignmentsResponse,
} from "./assignments.types";
import { baseApiSlice } from "@/lib/api/base.api";

export const assignmentApi =
  baseApiSlice.injectEndpoints({
    overrideExisting: true,

    endpoints: (builder) => ({
      getAllAssignments: builder.query<
        GetAssignmentsResponse,
        GetAssignmentsRequest | void
      >({
        query: (params) => {
          const searchParams = new URLSearchParams();

          searchParams.set(
            "pageNumber",
            String(params?.pageNumber ?? 1)
          );

          searchParams.set(
            "pageSize",
            String(params?.pageSize ?? 10)
          );

          if (params?.searchTerm) {
            searchParams.set(
              "searchTerm",
              params.searchTerm
            );
          }

          params?.state?.forEach((state) => {
            searchParams.append("state", state);
          });

          if (params?.sortBy) {
            searchParams.set("sortBy", params.sortBy);
          }

          if (params?.sortDirection) {
            searchParams.set(
              "sortDesc",
              String(
                params.sortDirection ===
                SortDirection.Desc
              )
            );
          }

          if (params?.assignedDate) {
            searchParams.set(
              "assignedDate",
              params.assignedDate
            );
          }

          if (params?.includeDeleted !== undefined) {
            searchParams.set(
              "includeDeleted",
              String(params.includeDeleted)
            );
          }

          return {
            url: `v1/assignments?${searchParams.toString()}`,
            method: "GET",
          };
        },

        providesTags: ["Assignment"],
      }),

      getAssignmentById: builder.query<
        AssignmentDetails,
        string
      >({
        query: (id) => ({
          url: `v1/assignments/${id}`,
          method: "GET",
        }),

        providesTags: (_result, _error, id) => [
          { type: "Assignment", id },
        ],
      }),

    }),
  });

export const {
  useGetAllAssignmentsQuery,
  useGetAssignmentByIdQuery
} = assignmentApi;
