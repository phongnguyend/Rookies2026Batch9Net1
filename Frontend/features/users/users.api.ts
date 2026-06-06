import { baseApiSlice } from "@/lib/api/base.api";
import {
  EditUserRequest,
  EditUserResponse,
  CreateUserRequest,
  CreateUserResponse,
  GetUserByIdResponse,
  GetUserForEditResponse,
  GetUsersRequest,
  GetUsersResponse,
  LookupUsers,
  CanDisableUser,
  DisableUser,
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
          ...(params.sortDesc !== undefined
            ? { sortDesc: params.sortDesc }
            : {}),
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

    createUser: builder.mutation<CreateUserResponse, CreateUserRequest>({
      query: (body) => ({
        url: "v1/users",
        method: "POST",
        body,
      }),
      invalidatesTags: ["Users"],
    }),

    lookupUsers: builder.query<LookupUsers.Response, LookupUsers.Request>({
      query: (params) => ({
        url: "v1/users/lookup",
        params,
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

    canDisableUser: builder.query<CanDisableUser.Response, CanDisableUser.Request>({
      query: ({ targetUserId }) => ({
        url: `v1/users/${targetUserId}/can-disable`,
        method: "GET",
      }),
      providesTags: ["Users"],
    }),

    disableUser: builder.mutation<DisableUser.Response, DisableUser.Request>({
      query: ({ targetUserId }) => ({
        url: `v1/users/${targetUserId}/disable`,
        method: "PATCH",
      }),
      invalidatesTags: ["Users"],
    }),
  }),
});

export const {
  useGetUsersQuery,
  useGetUserByIdQuery,
  useCreateUserMutation,
  useLookupUsersQuery,
  useEditUserMutation,
  useGetUserForEditQuery,
  useCanDisableUserQuery,
  useDisableUserMutation,
} = usersApi;
