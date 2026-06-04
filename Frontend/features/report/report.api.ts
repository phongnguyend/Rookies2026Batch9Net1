import { baseApiSlice } from "@/lib/api/base.api";
import { ViewReport, Export, CurrentDownload, Cancel } from "./report.types";

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
    getExportStatus: builder.query<CurrentDownload.Response, void>({
      query: () => ({
        url: "v1/report/export",
        method: "GET",
      }),
    }),
    startExport: builder.mutation<Export.Response, Export.Request>({
      query: (params) => ({
        url: "v1/report/export",
        method: "POST",
        params: {
          sortBy: params?.sortBy,
          sortDirection: params?.sortDirection,
        },
      }),
    }),
    cancelExport: builder.mutation<Cancel.Response, void>({
      query: () => ({
        url: "v1/report/export",
        method: "DELETE",
      }),
    }),
  }),
});

export const {
  useGetReportQuery,
  useLazyGetReportQuery,
  useGetExportStatusQuery,
  useStartExportMutation,
  useCancelExportMutation,
} = reportApi;
