import type {
  GetUserByIdResponse,
  GetUsersRequest,
  GetUsersResponse,
  User,
} from './test.types';
import { baseApiSlice } from '@/lib/api/base.api';

export const testApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({
    getUsers: builder.query<GetUsersResponse, GetUsersRequest | void>({
      query: () => ({
        url: '/users',
        method: 'GET',
      }),

      transformResponse: (response: User[], _meta, arg) => {
        const pageNumber = arg?.page ?? 1;
        const pageSize = arg?.limit ?? 10;
        const search = arg?.search?.trim().toLowerCase() ?? '';
        const skins = arg?.skins ?? [];
        const createdDate = arg?.createdDate ?? null;

        let filteredUsers = response;

        if (search) {
          filteredUsers = filteredUsers.filter((user) =>
            [
              user.name,
              user.city,
              user.cityName,
              user.country,
              user.address,
              user.skin,
            ].some((value) => value.toLowerCase().includes(search)),
          );
        }

        if (skins.length > 0) {
          filteredUsers = filteredUsers.filter((user) =>
            skins.includes(user.skin),
          );
        }

        if (createdDate) {
          filteredUsers = filteredUsers.filter((user) => {
            const userDate = new Date(user.createdAt);

            return userDate.toDateString() === createdDate.toDateString();
          });
        }

        const totalCount = filteredUsers.length;
        const totalPages = Math.ceil(totalCount / pageSize) || 1;
        const startIndex = (pageNumber - 1) * pageSize;
        const items = filteredUsers.slice(startIndex, startIndex + pageSize);

        return {
          items,
          pageNumber,
          totalPages,
          totalCount,
          pageSize,
          hasPreviousPage: pageNumber > 1,
          hasNextPage: pageNumber < totalPages,
        };
      },

      providesTags: ['Users'],
    }),

    getUserById: builder.query<GetUserByIdResponse, string>({
      query: (id) => ({
        url: `/users/${id}`,
        method: 'GET',
      }),
      providesTags: (_result, _error, id) => [{ type: 'Users', id }],
    }),
  }),
});

export const { useGetUsersQuery, useGetUserByIdQuery } = testApi;