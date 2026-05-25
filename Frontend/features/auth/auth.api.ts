import { baseApiSlice } from "@/lib/api/base.api";
import { Login, Refresh, GetMe, FirstChangePassword } from "./auth.types";

export const authApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({
    login: builder.mutation<Login.Response, Login.Request>({
      query: (credentials) => ({
        url: "/v1/auth/login",
        method: "POST",
        body: credentials,
      }),
    }),

    refresh: builder.mutation<Refresh.Response, Refresh.Request>({
      query: () => ({
        url: "/auth/refresh",
        method: "POST",
      }),
    }),

    getMe: builder.query<GetMe.Response, void>({
      query: () => ({
        url: "/v1/auth/profile",
        method: "GET",
      }),
    }),

    firstChangePassword: builder.mutation<FirstChangePassword.Response, FirstChangePassword.Request>({
      query: (data) => ({
        url: "/v1/auth/first-change-password",
        method: "POST",
        body: data,
      }),
    }),
  }),
});

export const {
  useLoginMutation,
  useRefreshMutation,
  useLazyGetMeQuery,
  useGetMeQuery,
  useFirstChangePasswordMutation,
} = authApi;
