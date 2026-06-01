import { baseApiSlice } from "@/lib/api/base.api";
import type {
  GetAssetsRequest,
  GetAssetsResponse,
  GetAssetDetailResponse,
  GetCategoriesResponse,
  CreateAssetRequest,
  CreateAssetResponse,
  CreateCategoryRequest,
  CreateCategoryResponse,
} from "./assets.types";

export const assetsApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({
    getAssets: builder.query<GetAssetsResponse, GetAssetsRequest | void>({
      query: (params) => ({
        url: "v1/assets",
        method: "GET",
        params: {
          categories: params?.categories?.join(","),
          states: params?.states?.join(","),
          search: params?.search,
          sortBy: params?.sortBy,
          sortDirection: params?.sortDirection,
          isCreatedNewAsset: params?.isCreatedNewAsset ?? false,
          pageNumber: params?.pageNumber ?? 1,
          pageSize: params?.pageSize ?? 10,
        },
      }),
      providesTags: ["Asset"],
    }),

    // This Api should be moved to separate file, but for now we can keep it here since it's related to assets
    getCategories: builder.query<GetCategoriesResponse, void>({
      query: () => ({
        url: "v1/categories",
        method: "GET",
      }),
      providesTags: ["Asset"],
    }),

    getAssetById: builder.query<GetAssetDetailResponse, string>({
      query: (id) => ({
        url: `v1/assets/${id}`,
        method: "GET",
      }),
      providesTags: (_result, _error, id) => [{ type: "Asset", id }],
    }),

    createAsset: builder.mutation<CreateAssetResponse, CreateAssetRequest>({
      query: (body) => ({
        url: "v1/assets",
        method: "POST",
        body,
      }),
      invalidatesTags: ["Asset"],
    }),

    createCategory: builder.mutation<
      CreateCategoryResponse,
      CreateCategoryRequest
    >({
      query: (body) => ({
        url: "v1/categories",
        method: "POST",
        body,
      }),
      invalidatesTags: ["Asset"],
    }),

  }),
});

export const {
  useGetAssetsQuery,
  useGetAssetByIdQuery,
  useGetCategoriesQuery,
  useCreateAssetMutation,
  useCreateCategoryMutation,
} = assetsApi;
