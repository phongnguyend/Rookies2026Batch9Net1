import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { ENV_CONFIGS } from "../config/env";
import { ApiErrorResponse } from "./base.types";
import { BaseQueryFn } from "@reduxjs/toolkit/query";

const rawBaseQuery = fetchBaseQuery({
  baseUrl: ENV_CONFIGS.apiUrl,
  credentials: 'include', // send cookie automatic
});

const customBaseQuery: BaseQueryFn = async (args, api, extraOptions) => {
  const result = await rawBaseQuery(args, api, extraOptions);

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
