import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { ENV_CONFIGS } from "../config/env";
import { ApiErrorResponse } from "./base.types";
import { BaseQueryFn } from "@reduxjs/toolkit/query";
import { logout } from "@/features/auth/auth.slice";


const rawBaseQuery = fetchBaseQuery({
  baseUrl: ENV_CONFIGS.apiUrl,
  credentials: 'include', // send cookie automatic
});

let refreshPromise: Promise<any> | null = null;

const customBaseQuery: BaseQueryFn = async (args, api, extraOptions) => {
  let result = await rawBaseQuery(args, api, extraOptions);

  const isRefreshRequest = typeof args === "string"
    ? args === "/auth/refresh"
    : (args as { url?: string })?.url === "/auth/refresh";

  // automaticaly call the /auth/refresh when ever the access token expired
  if (result.error?.status == 401 && !isRefreshRequest) {
    if (!refreshPromise) {
      refreshPromise = (async () => {
        return await rawBaseQuery(
          {
            url: "/auth/refresh",
            method: "POST"
          },
          api,
          extraOptions
        );
      })();
    }

    const refreshResult = await refreshPromise;
    refreshPromise = null;

    // if refresh successful (no error), then continue with result query
    if (!refreshResult.error) {
      result = await rawBaseQuery(args, api, extraOptions);
    } else {
      api.dispatch(logout());
      return result;
    }
  }


  // remapping error object to have a response format like in server
  if (result.error) {
    const rawErrordata = result.error.data as
      | Record<string, unknown>
      | undefined;

    const customErrorResponse: ApiErrorResponse = {
      title: (rawErrordata?.title as string) || "An error occurred",
      type: (rawErrordata?.type as string) || "Unknown",
      status: Number(result.error.status) || 500,
      detail:
        (rawErrordata?.detail as string) ||
        "Something went wrong. Please try again later.",
      errors: (rawErrordata?.errors as ApiErrorResponse["errors"]) || undefined,
    };

    return { error: customErrorResponse };
  }

  return result;
};

export const baseApiSlice = createApi({
  reducerPath: "api",
  baseQuery: customBaseQuery,
  tagTypes: ["Account", "Asset", "Assignment", "Report", "Return", "Users"],
  endpoints: () => ({}),
});
