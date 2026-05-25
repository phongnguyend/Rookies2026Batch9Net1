import { baseApiSlice } from "@/lib/api/base.api";
import type {
  GetAssetsRequest,
  GetAssetsResponse,
  GetAssetDetailResponse,
  GetCategoriesResponse,
} from "./assets.types";

export const assetsApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({

    getAssets: builder.query<GetAssetsResponse, GetAssetsRequest | void>({
      query: (params) => ({
        url: "v1/assets",
        method: "GET",
        params: {
          categories: params?.categories?.join(","),  // ← join array to string
          states: params?.states?.join(","),
          search: params?.search,        // ← join array to string
          pageNumber: params?.pageNumber ?? 1,
          pageSize: params?.pageSize ?? 10,
        },
      }),
      providesTags: ["Asset"],
    }),

    getCategories: builder.query<GetCategoriesResponse, void>({
      query: () => ({
        url: "v1/categories",
        method: "GET",
      }),
      providesTags: ["Asset"],
    }),

    getAssetById: builder.query<GetAssetDetailResponse, string>({
      query: (id) => ({
        url: `/assets/${id}`,
        method: "GET",
      }),
      providesTags: (_result, _error, id) => [{ type: "Asset", id }],
    }),

  }),
});

// ← these are the hooks your page imports
export const {
  useGetAssetsQuery,
  useGetAssetByIdQuery,
  useGetCategoriesQuery
} = assetsApi;
