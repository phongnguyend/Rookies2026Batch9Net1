import { baseApiSlice } from "@/lib/api/base.api";
import {
  EditUserRequest,
  EditUserResponse,
  GetUserByIdResponse,
  GetUserForEditResponse,
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
    getUserForEdit: builder.query<GetUserForEditResponse, string>({
      query: (id) => ({
        url: `v1/users/${id}/edit`,
        method: "GET",
      }),
      providesTags: ["Users"],
    }),
    editUser: builder.mutation<EditUserResponse, EditUserRequest>({
      query: ({ userId, ...body }) => ({
        url: `v1/users/${userId}/edit`,
        method: "PUT",
        body,
      }),
      invalidatesTags: ["Users"],
    }),
  }),
});

export const {
  useEditUserMutation,
  useGetUserForEditQuery,
  useGetUserByIdQuery,
  useGetUsersQuery,
} = usersApi;
