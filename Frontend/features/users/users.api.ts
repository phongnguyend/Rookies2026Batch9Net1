import { baseApiSlice } from "@/lib/api/base.api";
import {
  GetUserByIdResponse,
  GetUsersRequest,
  GetUsersResponse,
} from "./users.types";

export const usersApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,
  endpoints: (builder) => ({
    getUsers: builder.query<GetUsersResponse, GetUsersRequest>({
      query: (params) => ({
        url: "v1/users",
        method: "GET",
        params: {
          pageNumber: params.pageNumber,
          pageSize: params.pageSize,
          ...(params.searchTerm ? { searchTerm: params.searchTerm } : {}),
          ...(params.type ? { type: params.type } : {}),
          ...(params.sortBy ? { sortBy: params.sortBy } : {}),
          ...(params.sortDesc !== undefined ? { sortDesc: params.sortDesc } : {}),
        },
      }),
      providesTags: ["Users"],
    }),
    getUserById: builder.query<GetUserByIdResponse, string>({
      query: (id) => ({
        url: `v1/users/${id}`,
        method: "GET",
      }),
      providesTags: ["Users"],
    }),
  }),
});

export const { useGetUsersQuery, useGetUserByIdQuery } = usersApi;
