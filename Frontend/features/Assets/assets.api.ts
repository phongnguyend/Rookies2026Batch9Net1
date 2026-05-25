import { baseApiSlice } from "@/lib/api/base.api";
import type {
  GetAssetsRequest,
  GetAssetsResponse,
  GetAssetDetailResponse,
} from "./assets.types";

export const assetsApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({

    getAssets: builder.query<GetAssetsResponse, GetAssetsRequest | void>({
      query: (params) => ({
        url: "/assets",
        method: "GET",
        params: {
          category: params?.category,
          state: params?.state,
          pageNumber: params?.pageNumber ?? 1,
          pageSize: params?.pageSize ?? 10,
        },
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
} = assetsApi;
