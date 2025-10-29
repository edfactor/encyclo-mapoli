import { createApi } from "@reduxjs/toolkit/query/react";
import {
  AccountHistoryReportRequest,
  AccountHistoryReportResponse,
  ReportResponseBase
} from "../../types/reports/AccountHistoryReportTypes";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const AccountHistoryReportApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "accountHistoryReportApi",
  endpoints: (builder) => ({
    getAccountHistoryReport: builder.query<
      ReportResponseBase<AccountHistoryReportResponse>,
      AccountHistoryReportRequest
    >({
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

export const { useGetAccountHistoryReportQuery } = AccountHistoryReportApi;
