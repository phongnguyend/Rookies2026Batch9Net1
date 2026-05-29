import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { ENV_CONFIGS } from "../config/env";
import { ApiErrorResponse } from "./base.types";
import { BaseQueryFn, FetchArgs, FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { logoutAccount } from "@/features/auth/auth.slice";

const rawBaseQuery = fetchBaseQuery({
  baseUrl: ENV_CONFIGS.apiUrl + "/api",
  credentials: 'include', // send cookie automatic

  // fallback to send access token to the server
  prepareHeaders: (headers) => {
    if (typeof window !== "undefined") {
      const token = localStorage.getItem("accessToken");
      if (token) {
        headers.set("Authorization", `Bearer ${token}`);
      }
    }
    return headers;
  },
});

let refreshPromise: ReturnType<typeof rawBaseQuery> | null = null;

// Skip refresh token calling for those route
// - /auth/login (login fails with 401 should show credential errors, not refresh errors)
// - /auth/refresh (cannot refresh a refresh request)
// - /auth/logout (logout fails with 401 should just logout cleanly)
const shouldSkipRefresh = (args: string | FetchArgs): boolean => {
  const url = typeof args === "string" ? args : args.url;
  return url.includes("/login") || url.includes("/refresh") || url.includes("/logout");
}

const refreshToken = async (
  api: any,
  extraOptions: any,
) => {

  // If no refreshIs current requested, calling one
  if (!refreshPromise) {

    // fallback to get refersh token from the localStorage
    const storedRefreshToken = typeof window !== "undefined" ? localStorage.getItem("refreshToken") : null;
    const headers: Record<string, string> = {};
    if (storedRefreshToken) {
      headers["X-Refresh-Token"] = storedRefreshToken;
    }

    refreshPromise = rawBaseQuery(
      {
        url: "/auth/refresh",
        method: "POST",
        headers,
      },
      api,
      extraOptions,
    );
  }

  const result = await refreshPromise;
  refreshPromise = null;

  if (result.data) {
    const data = result.data as any;
    if (data.accessToken && data.refreshToken) {
      if (typeof window !== "undefined") {
        localStorage.setItem("accessToken", data.accessToken);
        localStorage.setItem("refreshToken", data.refreshToken);
      }
    }
  }

  return result;
};

// Error Response Mapping
// - Mapping errors from server to client
const mapErrorResponse = (
  error: FetchBaseQueryError,
): ApiErrorResponse => {
  const rawErrorData =
    (error.data as Record<string, unknown>) || {};

  return {
    title:
      (rawErrorData.title as string) ||
      "An error occurred",

    type:
      (rawErrorData.type as string) ||
      "Unknown",

    status:
      Number(error.status) || 500,

    detail:
      (rawErrorData.detail as string) ||
      "Something went wrong. Please try again later.",

    errors:
      rawErrorData.errors as ApiErrorResponse["errors"],
  };
};

// Handle Auth Failure
// - refresh token expired
// - refresh endpoint failed
const handleAuthFailure = (api: any) => {

  // Remove auth state, must know
  api.dispatch(logoutAccount())

  // Clear tokens from local storage
  if (typeof window !== "undefined") {
    localStorage.clear();
  }

  // Redirect user to login form if not already on the login page
  if (typeof window !== "undefined") {
    const currentPath = window.location.pathname;
    if (currentPath !== "/" && currentPath !== "/index.html" && currentPath !== "/login") {
      window.location.replace("/");
    }
  }
}

// ================= Base Query + Refresh Token Revocation + Access Token Revocation ==============

const customBaseQuery: BaseQueryFn<
  string | FetchArgs,
  unknown,
  ApiErrorResponse
> = async (
  args,
  api,
  extraOptions,
) => {

    // Rule 0: Run original request (e.g. /categories)
    let result = await rawBaseQuery(
      args,
      api,
      extraOptions
    );

    // Rule 1: Access Token expired
    // - response 401 Authorized, then try to call the refresh token
    if (result.error?.status === 401 && !shouldSkipRefresh(args)) {
      const refreshTokenResult = await refreshToken(api, extraOptions);

      // if refreshToken return is still valid
      if (!refreshTokenResult.error) {
        result = await rawBaseQuery(args, api, extraOptions);
      }

      // if refrsth token also invalid
      else {
        handleAuthFailure(api);
        return {
          error: mapErrorResponse(refreshTokenResult.error)
        }
      }
    }


    // Rule 2: Continue with original response, Normalize API errors
    if (result.error) {
      return {
        error: mapErrorResponse(
          result.error,
        ),
      };
    }

    // Rule 3: If no error, just continue the original request
    return result;
  }

export const baseApiSlice = createApi({
  reducerPath: "api",
  baseQuery: customBaseQuery,
  tagTypes: ["Account", "Asset", "Assignment", "Report", "Return", "Users"],
  endpoints: () => ({}),
});
