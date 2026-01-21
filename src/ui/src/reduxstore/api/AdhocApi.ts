import { createApi } from "@reduxjs/toolkit/query/react";
import { convertToISODateString } from "../../utils/dateUtils";

import {
  clearBreakdownByStoreTotals,
  clearBreakdownGrandTotals,
  setBreakdownByStore,
  setBreakdownByStoreManagement,
  setBreakdownByStoreTotals,
  setBreakdownGrandTotals,
  setRecentlyTerminated,
  setTerminatedLetters
} from "reduxstore/slices/yearsEndSlice";
import {
  AdhocBeneficiariesReportRequest,
  adhocBeneficiariesReportResponse,
  BreakdownByStoreAndDateRangeRequest,
  BreakdownByStoreEmployee,
  BreakdownByStoreRequest,
  BreakdownByStoreTotals,
  ForfeitureAdjustmentDetail,
  ForfeitureAdjustmentUpdateRequest,
  GrandTotalsByStoreResponseDto,
  PagedReportResponse,
  ProfitYearRequest,
  QPAY066BTerminatedWithVestedBalanceRequest,
  QPAY066BTerminatedWithVestedBalanceResponse,
  RecentlyTerminatedResponse,
  StartAndEndDateRequest,
  SuggestedForfeitResponse,
  SuggestForfeitureAdjustmentRequest,
  TerminatedLettersRequest,
  TerminatedLettersResponse
} from "reduxstore/types";
import {
  AccountHistoryReportPaginatedResponse,
  AccountHistoryReportRequest
} from "../../types/reports/AccountHistoryReportTypes";
import { createDataSourceAwareBaseQuery } from "./api";

import {
  clearForfeitureAdjustmentData,
  setForfeitureAdjustmentData
} from "reduxstore/slices/forfeituresAdjustmentSlice";

/* Use the centralized data source aware base query
   2-minute timeout for adhoc reports (consistent with year-end reports) */
const baseQuery = createDataSourceAwareBaseQuery(120000); // 2 minutes in milliseconds
/* ------------------------------------------------------------------------- */

export const AdhocApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "adhocApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0, // Remove data immediately when no longer in use
  refetchOnMountOrArgChange: true, // Always fetch fresh data
  endpoints: (builder) => ({
    getRecentlyTerminatedReport: builder.query<RecentlyTerminatedResponse, StartAndEndDateRequest>({
      query: (params) => {
        return {
          url: "ad-hoc/terminated-employees-report",
          method: "GET",
          params: {
            profitYear: params.profitYear,
            beginningDate: params.beginningDate,
            endingDate: params.endingDate,
            excludeZeroBalance: params.excludeZeroBalance,
            take: params.pagination.take,
            skip: params.pagination.skip,
            sortBy: params.pagination.sortBy,
            isSortDescending: params.pagination.isSortDescending
          }
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setRecentlyTerminated(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getTerminatedLettersReport: builder.query<TerminatedLettersResponse, TerminatedLettersRequest>({
      query: (params) => {
        return {
          url: "ad-hoc/terminated-employees-report-needing-letter",
          method: "GET",
          params: {
            profitYear: params.profitYear,
            beginningDate: params.beginningDate,
            endingDate: params.endingDate,
            excludeZeroBalance: params.excludeZeroBalance,
            badgeNumbers: params.badgeNumbers,
            take: params.pagination.take,
            skip: params.pagination.skip,
            sortBy: params.pagination.sortBy,
            isSortDescending: params.pagination.isSortDescending
          }
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setTerminatedLetters(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getTerminatedLettersDownload: builder.query<Blob, TerminatedLettersRequest>({
      query: (params) => ({
        url: "ad-hoc/terminated-employees-report-needing-letter/download",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          beginningDate: params.beginningDate,
          endingDate: params.endingDate,
          excludeZeroBalance: params.excludeZeroBalance,
          badgeNumbers: params.badgeNumbers,
          isXerox: params.isXerox,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        },
        responseHandler: (response) => response.blob()
      })
    }),
    getForfeitureAdjustments: builder.query<
      SuggestedForfeitResponse,
      SuggestForfeitureAdjustmentRequest & { onlyNetworkToastErrors?: boolean }
    >({
      query: (params) => {
        const { onlyNetworkToastErrors, ...requestParams } = params;

        const ssnDigits = requestParams.ssn ? requestParams.ssn.replace(/\D/g, "") : "";
        const badgeDigits = requestParams.badge ? requestParams.badge.replace(/\D/g, "") : "";
        const ssn = ssnDigits.length > 0 ? Number.parseInt(ssnDigits, 10) : undefined;
        const badge = badgeDigits.length > 0 ? Number.parseInt(badgeDigits, 10) : undefined;

        return {
          url: "ad-hoc/forfeiture-adjustments",
          method: "POST",
          body: {
            ssn,
            badge
          },
          meta: { onlyNetworkToastErrors }
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setForfeitureAdjustmentData(data));
        } catch (err: unknown) {
          // Always clear the data on any error
          dispatch(clearForfeitureAdjustmentData());

          // Don't handle "Employee not found" errors here - let them bubble up to component
          type RTKQueryError = {
            error?: {
              status?: number;
              data?: {
                title?: string;
              };
            };
          };

          if (
            typeof err === "object" &&
            err !== null &&
            "error" in err &&
            typeof (err as RTKQueryError).error === "object" &&
            (err as RTKQueryError).error?.status === 500 &&
            (err as RTKQueryError).error?.data?.title === "Employee not found."
          ) {
            return; // Don't log or handle, just clear data and let component handle it
          }

          // Handle other errors as before
          console.log("Err: " + String(err));
        }
      }
    }),
    updateForfeitureAdjustment: builder.mutation<
      ForfeitureAdjustmentDetail,
      ForfeitureAdjustmentUpdateRequest & { suppressAllToastErrors?: boolean; onlyNetworkToastErrors?: boolean }
    >({
      query: (params) => {
        const { suppressAllToastErrors, onlyNetworkToastErrors, ...requestData } = params;
        return {
          url: "ad-hoc/forfeiture-adjustments/update",
          method: "PUT",
          body: requestData,
          // Pass params through meta so middleware can access it
          meta: { suppressAllToastErrors, onlyNetworkToastErrors }
        };
      }
    }),

    updateForfeitureAdjustmentBulk: builder.mutation<ForfeitureAdjustmentDetail[], ForfeitureAdjustmentUpdateRequest[]>(
      {
        query: (params) => ({
          url: "ad-hoc/forfeiture-adjustments/bulk-update",
          method: "PUT",
          body: params
        })
      }
    ),

    adhocBeneficiariesReport: builder.query<adhocBeneficiariesReportResponse, AdhocBeneficiariesReportRequest>({
      query: (params) => ({
        url: "ad-hoc/beneficiaries-report",
        method: "GET",
        params: {
          isAlsoEmployee: params.isAlsoEmployee,
          profitYear: params.profitYear,
          skip: params.skip || 0,
          take: params.take || 255,
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { dispatch: _dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    // **************************
    // Breakdown By Store Queries
    //.  with PagedReportResponse<BreakdownByStoreEmployee>
    // **************************
    // 1. Get Breakdown By Store

    getBreakdownByStore: builder.query<PagedReportResponse<BreakdownByStoreEmployee>, BreakdownByStoreRequest>({
      query: (params) => ({
        url: "ad-hoc/stores/breakdown",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getBreakdownByStoreInactive: builder.query<PagedReportResponse<BreakdownByStoreEmployee>, BreakdownByStoreRequest>({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/inactive",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreInactiveWithVestedBalance: builder.query<
      PagedReportResponse<BreakdownByStoreEmployee>,
      BreakdownByStoreRequest
    >({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/inactive/withvestedbalance",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreTerminatedVestedBalance: builder.query<
      PagedReportResponse<BreakdownByStoreEmployee>,
      BreakdownByStoreAndDateRangeRequest
    >({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/terminated/withvestedbalance",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          startDate: params.startDate,
          endDate: params.endDate,
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreTerminatedBalanceNotVested: builder.query<
      PagedReportResponse<BreakdownByStoreEmployee>,
      BreakdownByStoreAndDateRangeRequest
    >({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/terminated/withcurrentbalance/notvested",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          // Convert dates from MM/dd/yyyy to yyyy-MM-dd for API
          startDate: convertToISODateString(params.startDate),
          endDate: convertToISODateString(params.endDate),
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreTerminatedWithBenAllocations: builder.query<
      PagedReportResponse<BreakdownByStoreEmployee>,
      BreakdownByStoreRequest
    >({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/terminated/withbeneficiaryallocation",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          // Convert dates from MM/dd/yyyy to yyyy-MM-dd for API
          startDate: convertToISODateString(params.startDate),
          endDate: convertToISODateString(params.endDate),
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreTerminatedWithBalanceActivity: builder.query<
      PagedReportResponse<BreakdownByStoreEmployee>,
      BreakdownByStoreAndDateRangeRequest
    >({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/terminated/withbalanceactivity",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          startDate: params.startDate,
          endDate: params.endDate,
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreRetiredWithBalanceActivity: builder.query<
      PagedReportResponse<BreakdownByStoreEmployee>,
      BreakdownByStoreRequest
    >({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/retired/withbalanceactivity",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          // Convert dates from MM/dd/yyyy to yyyy-MM-dd for API
          startDate: convertToISODateString(params.startDate),
          endDate: convertToISODateString(params.endDate),
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;

          // Use the storeManagement flag to determine where to store the data
          if (arg.storeManagement) {
            dispatch(setBreakdownByStoreManagement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreMonthly: builder.query<PagedReportResponse<BreakdownByStoreEmployee>, BreakdownByStoreRequest>({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/monthly",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          // Convert dates from MM/dd/yyyy to yyyy-MM-dd for API
          startDate: convertToISODateString(params.startDate),
          endDate: convertToISODateString(params.endDate),
          employeeName: params.employeeName,
          badgeNumber: params.badgeNumber,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          // Monthly employees report uses breakdownByStore slice
          dispatch(setBreakdownByStore(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreTotals: builder.query<BreakdownByStoreTotals, BreakdownByStoreRequest>({
      query: (params) => ({
        url: `ad-hoc/stores/${params.storeNumber}/breakdown/totals`,
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(clearBreakdownByStoreTotals());
          const { data } = await queryFulfilled;
          dispatch(setBreakdownByStoreTotals(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearBreakdownByStoreTotals());
        }
      }
    }),
    getBreakdownGrandTotals: builder.query<GrandTotalsByStoreResponseDto, ProfitYearRequest>({
      query: (params) => ({
        url: "ad-hoc/stores/breakdown/totals",
        method: "GET",
        params: {
          profitYear: params.profitYear
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(clearBreakdownByStoreTotals());
          const { data } = await queryFulfilled;
          dispatch(setBreakdownGrandTotals(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearBreakdownGrandTotals());
        }
      }
    }),
    getQPAY066BTerminatedWithVestedBalance: builder.query<
      QPAY066BTerminatedWithVestedBalanceResponse,
      QPAY066BTerminatedWithVestedBalanceRequest
    >({
      query: (params) => ({
        url: "ad-hoc/breakdown-by-store/terminated/withcurrentbalance/notvested",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          skip: params.pagination.skip,
          take: params.pagination.take,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      })
    }),

    // Account History Report (Divorce Report) endpoints
    getAccountHistoryReport: builder.query<AccountHistoryReportPaginatedResponse, AccountHistoryReportRequest>({
      query: (params) => {
        return {
          url: "/ad-hoc/divorce-report",
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
    }),
    downloadAccountHistoryReportPdf: builder.mutation<Blob, Omit<AccountHistoryReportRequest, "pagination">>({
      query: (params) => ({
        url: "/ad-hoc/divorce-report/export-pdf",
        method: "POST",
        body: {
          badgeNumber: params.badgeNumber,
          startDate: params.startDate,
          endDate: params.endDate
        },
        responseHandler: (response) => response.blob()
      })
    })
  })
});

export const {
  useLazyAdhocBeneficiariesReportQuery,
  useLazyGetBreakdownByStoreQuery,
  useLazyGetBreakdownByStoreTotalsQuery,
  useLazyGetBreakdownGrandTotalsQuery,
  useLazyGetBreakdownByStoreInactiveQuery,
  useLazyGetBreakdownByStoreInactiveWithVestedBalanceQuery,
  useLazyGetBreakdownByStoreTerminatedVestedBalanceQuery,
  useLazyGetBreakdownByStoreTerminatedBalanceNotVestedQuery,
  useLazyGetBreakdownByStoreTerminatedWithBenAllocationsQuery,
  useLazyGetBreakdownByStoreTerminatedWithBalanceActivityQuery,
  useLazyGetBreakdownByStoreRetiredWithBalanceActivityQuery,
  useLazyGetBreakdownByStoreMonthlyQuery,
  useLazyGetForfeitureAdjustmentsQuery,
  useLazyGetQPAY066BTerminatedWithVestedBalanceQuery,
  useLazyGetRecentlyTerminatedReportQuery,
  useLazyGetTerminatedLettersDownloadQuery,
  useLazyGetTerminatedLettersReportQuery,
  useUpdateForfeitureAdjustmentBulkMutation,
  useUpdateForfeitureAdjustmentMutation,
  useLazyGetAccountHistoryReportQuery,
  useDownloadAccountHistoryReportPdfMutation
} = AdhocApi;
