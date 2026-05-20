import { createApi, fakeBaseQuery } from '@reduxjs/toolkit/query/react';
import type {
  GetUserByIdResponse,
  GetUsersRequest,
  GetUsersResponse,
  User,
} from './test.types';
import { fakeUsers } from './test.types';

export const testApi = createApi({
  reducerPath: 'testApi',
  baseQuery: fakeBaseQuery(),
  tagTypes: ['Users'],
  endpoints: (builder) => ({
    getUsers: builder.query<GetUsersResponse, GetUsersRequest | void>({
      queryFn: async (arg) => {
        const page = arg?.page ?? 1;
        const limit = arg?.limit ?? 10;
        const search = arg?.search?.trim().toLowerCase() ?? '';

        const filteredUsers = search
          ? fakeUsers.filter((user) =>
              [
                user.name,
                user.city,
                user.cityName,
                user.country,
                user.address,
                user.skin,
              ].some((value) => value.toLowerCase().includes(search)),
            )
          : fakeUsers;

        const totalItems = filteredUsers.length;
        const totalPages = Math.ceil(totalItems / limit);
        const startIndex = (page - 1) * limit;
        const data = filteredUsers.slice(startIndex, startIndex + limit);

        return {
          data: {
            data,
            paging: {
              page,
              limit,
              totalItems,
              totalPages,
              hasNextPage: page < totalPages,
              hasPreviousPage: page > 1,
            },
          },
        };
      },
      providesTags: ['Users'],
    }),

    getUserById: builder.query<GetUserByIdResponse, string>({
      queryFn: async (id) => {
        const user = fakeUsers.find((user) => user.id === id);

        if (!user) {
          return {
            error: {
              status: 404,
              data: 'User not found',
            },
          };
        }

        return {
          data: user,
        };
      },
      providesTags: (_result, _error, id) => [{ type: 'Users', id }],
    }),
  }),
});

export const { useGetUsersQuery, useGetUserByIdQuery } = testApi;