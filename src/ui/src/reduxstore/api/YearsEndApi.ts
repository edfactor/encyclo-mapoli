import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { prepareHeaders, url } from "./api";
import { RootState } from "reduxstore/store";
import { DemographicBadgesNotInPayprofitRequestDto, DemographicBadgesNotInPayprofitResponse, DuplicateSSNDetail, DuplicateSSNsRequestDto, NegativeEtvaForSSNsOnPayProfit, NegativeEtvaForSSNsOnPayprofitRequestDto, PagedReportResponse } from "reduxstore/types";
import { setDemographicBadgesNotInPayprofitData, setDuplicateSSNsData, setNegativeEtvaForSssnsOnPayprofit } from "reduxstore/slices/yearsEndSlice";
export const YearsEndApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: "https://localhost:7141/api/",
    prepareHeaders: (headers, { getState }) => {
      const token = (getState() as RootState).security.token;
      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }
      headers.set("impersonation", "Profit-Sharing-Administrator");
      
      return headers;
    }
  }),
  reducerPath: "yearsEndApi",
  endpoints: (builder) => ({
    getDuplicateSSNs: builder.query<PagedReportResponse<DuplicateSSNDetail>, DuplicateSSNsRequestDto>({
      query: (params) => ({
        url: `yearend/duplicate-ssns`,
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          profitYear: params.profitYear,
          impersonation: params.impersonation
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D Dupes " + JSON.stringify(data));
          dispatch(setDuplicateSSNsData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getDemographicBadgesNotInPayprofit: builder.query<DemographicBadgesNotInPayprofitResponse, DemographicBadgesNotInPayprofitRequestDto>({
      query: (params) => ({
        url: `yearend/demographic-badges-not-in-payprofit`,
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          impersonation: params.impersonation
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
          dispatch(setDemographicBadgesNotInPayprofitData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getDuplicateNamesAndBirthdays: builder.query({
      query: () => ({
        url: "yearend/duplicate-names-and-birthdays",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getMilitaryAndRehire: builder.query({
      query: () => ({
        url: "yearend/military-and-rehire",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getMilitaryAndRehireForfeitures: builder.query({
      query: () => ({
        url: "yearend/military-and-rehire-forfeitures",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getMilitaryAndRehireProfitSummary: builder.query({
      query: () => ({
        url: "yearend/military-and-rehire-profit-summary",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getMismatchedSSNsPayprofitAndDemoOnSameBadge: builder.query({
      query: () => ({
        url: "yearend/mismatched-ssns-payprofit-and-demo-on-same-badge",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getNamesMissingCommas: builder.query({
      query: () => ({
        url: "yearend/names-missing-commas",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getNegativeEVTASSN: builder.query<PagedReportResponse<NegativeEtvaForSSNsOnPayProfit>, NegativeEtvaForSSNsOnPayprofitRequestDto>({
      query: (params) => ({
        url: "yearend/negative-evta-ssn",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setNegativeEtvaForSssnsOnPayprofit(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getPayprofitBadgeWithoutDemographics: builder.query({
      query: () => ({
        url: "yearend/payprofit-badges-without-demographics",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getPayrollDuplicateSSNsOnPayprofit: builder.query({
      query: () => ({
        url: "yearend/payroll-duplicate-ssns-on-payprofit",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getWagesCurrentYear: builder.query({
      query: () => ({
        url: "yearend/wages-current-year",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getWagesPreviousYear: builder.query({
      query: () => ({
        url: "yearend/wages-previous-year",
        method: "GET",
        params: {
          take: 25,
          skip: 0
        }
      }),
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("@D " + JSON.stringify(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    })
  })
});

export const {
  useLazyGetDemographicBadgesNotInPayprofitQuery,
  useLazyGetDuplicateSSNsQuery,
  useLazyGetDuplicateNamesAndBirthdaysQuery,
  useLazyGetMilitaryAndRehireForfeituresQuery,
  useLazyGetMilitaryAndRehireProfitSummaryQuery,
  useLazyGetMilitaryAndRehireQuery,
  useLazyGetMismatchedSSNsPayprofitAndDemoOnSameBadgeQuery,
  useLazyGetWagesCurrentYearQuery,
  useLazyGetNamesMissingCommasQuery,
  useLazyGetNegativeEVTASSNQuery,
  useLazyGetPayprofitBadgeWithoutDemographicsQuery,
  useLazyGetPayrollDuplicateSSNsOnPayprofitQuery,
  useLazyGetWagesPreviousYearQuery
} = YearsEndApi;
