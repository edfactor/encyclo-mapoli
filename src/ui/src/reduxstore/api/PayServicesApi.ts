import { createApi } from "@reduxjs/toolkit/query/react";
import type { ProfitYearRequest } from "../../types/common/api";
import type { PayServicesResponse } from "../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const PayServicesApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "payServicesApi",
  tagTypes: [
    "PayServicesPartTime",
    "PayServicesFullTimeSalary",
    "PayServicesFullTimeEightHolidays",
    "PayServicesFullTimeAccruedHolidays"
  ],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  refetchOnFocus: false,
  refetchOnReconnect: false,
  endpoints: (builder) => ({
    /**
     * Get pay services for part-time employees
     */
    getPartTimePayServices: builder.query<PayServicesResponse, ProfitYearRequest>({
      query: ({ profitYear }) => ({
        url: `/payservices/parttime/${profitYear}`,
        method: "GET"
      }),
      providesTags: ["PayServicesPartTime"],
      keepUnusedDataFor: 0
    }),

    /**
     * Get pay services for full-time employees with 8 paid holidays
     */
    getFullTimeEightPaidHolidaysPayServices: builder.query<PayServicesResponse, ProfitYearRequest>({
      query: ({ profitYear }) => ({
        url: `/payservices/fulltimeeightpaidholidays/${profitYear}`,
        method: "GET"
      }),
      providesTags: ["PayServicesFullTimeEightHolidays"],
      keepUnusedDataFor: 0
    }),

    /**
     * Get pay services for full-time employees with accrued paid holidays
     */
    getFullTimeAccruedPaidHolidaysPayServices: builder.query<PayServicesResponse, ProfitYearRequest>({
      query: ({ profitYear }) => ({
        url: `/payservices/fulltimeaccruedpaidholidays/${profitYear}`,
        method: "GET"
      }),
      providesTags: ["PayServicesFullTimeAccruedHolidays"],
      keepUnusedDataFor: 0
    }),

    /**
     * Get pay services for full-time employees with straight salary
     */
    getFullTimeStraightSalaryPayServices: builder.query<PayServicesResponse, ProfitYearRequest>({
      query: ({ profitYear }) => ({
        url: `/payservices/fulltimestraightsalary/${profitYear}`,
        method: "GET"
      }),
      providesTags: ["PayServicesFullTimeSalary"],
      keepUnusedDataFor: 0
    })
  })
});

export const {
  useGetPartTimePayServicesQuery,
  useGetFullTimeEightPaidHolidaysPayServicesQuery,
  useGetFullTimeAccruedPaidHolidaysPayServicesQuery,
  useGetFullTimeStraightSalaryPayServicesQuery,
  useLazyGetPartTimePayServicesQuery,
  useLazyGetFullTimeEightPaidHolidaysPayServicesQuery,
  useLazyGetFullTimeAccruedPaidHolidaysPayServicesQuery,
  useLazyGetFullTimeStraightSalaryPayServicesQuery
} = PayServicesApi;
