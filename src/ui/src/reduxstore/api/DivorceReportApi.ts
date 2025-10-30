import { createApi } from "@reduxjs/toolkit/query/react";
import {
  DivorceReportRequest,
  DivorceReportResponse,
  ReportResponseBase
} from "../../types/reports/DivorceReportTypes";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const DivorceReportApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "divorceReportApi",
  endpoints: (builder) => ({
    getDivorceReport: builder.query<ReportResponseBase<DivorceReportResponse>, DivorceReportRequest>({
      query: (params) => {
        return {
          url: "/adhoc/divorce-report",
          method: "POST",
          body: {
            badgeNumber: params.badgeNumber,
            startDate: params.startDate,
            endDate: params.endDate
          }
        };
      }
    })
  })
});

export const { useGetDivorceReportQuery } = DivorceReportApi;
