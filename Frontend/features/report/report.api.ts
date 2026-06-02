import { baseApiSlice } from "@/lib/api/base.api";
import { ViewReport } from "./report.types";

export const reportApi = baseApiSlice.injectEndpoints({
  overrideExisting: true,

  endpoints: (builder) => ({
    getReport: builder.query<ViewReport.Response, ViewReport.Request>({
      query: (params) => ({
        url: "v1/report",
        method: "GET",
        params: {
          sortBy: params?.sortBy,
          sortDirection: params?.sortDirection,
          pageNumber: params?.pageNumber ?? 1,
          pageSize: params?.pageSize ?? 10,
        },
      }),
      providesTags: ["Report"],
    }),
  }),
});

export const { useGetReportQuery, useLazyGetReportQuery } = reportApi;
