import { baseApiSlice } from "@/lib/api/base.api";
import { SortDirection } from "@/lib/api/base.types";
import {
  type CompleteReturnRequestRequest,
  type GetReturnRequestsRequest,
  type GetReturnRequestsResponse,
} from "./returns.types";

export const returnsApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({
    getReturnRequests: builder.query<
      GetReturnRequestsResponse,
      GetReturnRequestsRequest | void
    >({
      query: (params) => {
        const searchParams = new URLSearchParams();

        searchParams.set("pageNumber", String(params?.pageNumber ?? 1));
        searchParams.set("pageSize", String(params?.pageSize ?? 10));

        if (params?.searchTerm) {
          searchParams.set("searchTerm", params.searchTerm);
        }

        params?.states?.forEach((state) => {
          searchParams.append("states", state);
        });

        if (params?.returnedDate) {
          searchParams.set("returnedDate", params.returnedDate);
        }

        if (params?.sortBy) {
          searchParams.set("sortBy", params.sortBy);
        }

        if (params?.sortDirection) {
          searchParams.set(
            "sortDesc",
            String(params.sortDirection === SortDirection.Desc),
          );
        }

        return {
          url: `v1/return-requests?${searchParams.toString()}`,
          method: "GET",
        };
      },

      providesTags: ["Return"],
    }),

    completeReturnRequest: builder.mutation<void, CompleteReturnRequestRequest>({
      query: ({ returnRequestId }) => ({
        url: `v1/admin/return-requests/${returnRequestId}/complete`,
        method: "POST",
      }),

      // Change state "Assignment" to [Return], "Asset" to [Available], "Return" to [Completed]
      invalidatesTags: ["Return", "Assignment", "Asset"],
    }),
  }),
});

export const { 
  useGetReturnRequestsQuery,
  useCompleteReturnRequestMutation,
} = returnsApi;
