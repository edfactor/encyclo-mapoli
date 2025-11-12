import { createApi } from "@reduxjs/toolkit/query/react";
import {
  AccountHistoryReportRequest,
  AccountHistoryReportPaginatedResponse
} from "../../types/reports/AccountHistoryReportTypes";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const AccountHistoryReportApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "accountHistoryReportApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getAccountHistoryReport: builder.query<
      AccountHistoryReportPaginatedResponse,
      AccountHistoryReportRequest
    >({
      query: (params) => {
        return {
          url: "/adhoc/divorce-report",
          method: "POST",
          body: {
            badgeNumber: params.badgeNumber,
            startDate: params.startDate,
            endDate: params.endDate,
            skip: params.pagination.skip ?? 0,
            take: params.pagination.take ?? 25,
            sortBy: params.pagination.sortBy ?? "profitYear",
            isSortDescending: params.pagination.isSortDescending ?? true
          }
        };
      }
    })
  })
});

export const { useGetAccountHistoryReportQuery } = AccountHistoryReportApi;
