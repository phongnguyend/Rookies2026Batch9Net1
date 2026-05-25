import { baseApiSlice } from "@/lib/api/base.api";
import { GetUsersRequest, GetUsersResponse } from "./users.types";

export const usersApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,
  endpoints: (builder) => ({
    getUsers: builder.query<GetUsersResponse, GetUsersRequest>({
      query: (params) => ({
        url: "https://localhost:8081/api/v1/users",
        method: "GET",
        params: {
          pageNumber: params.pageNumber,
          pageSize: params.pageSize,
          ...(params.searchTerm ? { searchTerm: params.searchTerm } : {}),
          ...(params.type ? { type: params.type } : {}),
          ...(params.sortBy ? { sortBy: params.sortBy } : {}),
          ...(params.sortDesc ? { sortDesc: params.sortDesc } : {}),
        },
      }),
      providesTags: ["Users"],
    }),
  }),
});

export const { useGetUsersQuery } = usersApi;
