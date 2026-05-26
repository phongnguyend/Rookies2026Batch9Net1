import { baseApiSlice } from "@/lib/api/base.api";
import {
  UserCreateReturnRequest,
  ViewUserAssignmentDetail,
  ViewUserAssignments,
} from "./user-assignment.types";

export const userAssignmentApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,
  endpoints: (builder) => ({
    viewUserAssignments: builder.query<
      ViewUserAssignments.Response,
      ViewUserAssignments.Request
    >({
      query: (params) => ({
        url: "/v1/user/assignments",
        params,
      }),
      providesTags: ["Assignment"],
    }),

    viewUserAssignmentDetail: builder.query<
      ViewUserAssignmentDetail.Response,
      ViewUserAssignmentDetail.Request
    >({
      query: ({ assignmentId }) => ({
        url: `/v1/user/assignments/${assignmentId}/detail`,
      }),
      providesTags: ["Assignment"],
    }),

    userCreateReturnRequest: builder.mutation<
      void,
      UserCreateReturnRequest.Request
    >({
      query: ({ assignmentId }) => ({
        url: `/v1/user/assignments/${assignmentId}/return-request`,
        method: "POST",
      }),
      invalidatesTags: ["Assignment"],
    }),
  }),
});
