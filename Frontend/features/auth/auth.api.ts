import { baseApiSlice } from "@/lib/api/base.api";
import { Login, Refresh, GetMe, FirstChangePassword, ChangePassword, Logout } from "./auth.types";

export const authApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({
    login: builder.mutation<Login.Response, Login.Request>({
      query: (credentials) => ({
        url: "/v1/auth/login",
        method: "POST",
        body: credentials,
      }),
      async onQueryStarted(arg, { queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          if (data != null && data?.accessToken && data?.refreshToken) {
            localStorage.setItem("accessToken", data.accessToken);
            localStorage.setItem("refreshToken", data.refreshToken);
          }
        } catch (error) {
          console.error("Login mutation error storing tokens:", error);
        }
      },
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

    changePassword: builder.mutation<ChangePassword.Response, ChangePassword.Request>({
      query: (data) => ({
        url: "/v1/auth/change-password",
        method: "POST",
        body: data,
      }),
    }),

    logout: builder.mutation<Logout.Response, Logout.Request>({
      query: () => ({
        url: "/v1/auth/logout",
        method: "POST",
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
  useChangePasswordMutation,
  useLogoutMutation,
} = authApi;
