import { createApi } from "@reduxjs/toolkit/query/react";

import {
  addBadgeNumberToUpdateAdjustmentSummary,
  clearBreakdownByStoreTotals,
  clearBreakdownGrandTotals,
  clearCertificates,
  clearProfitMasterApply,
  clearProfitMasterRevert,
  clearProfitMasterStatus,
  clearProfitSharingEdit,
  clearProfitSharingLabels,
  clearProfitSharingUnder21Report,
  clearProfitSharingUpdate,
  clearUnder21BreakdownByStore,
  clearUnder21Inactive,
  clearUnder21Totals,
  clearYearEndProfitSharingReportFrozen,
  clearYearEndProfitSharingReportLive,
  clearYearEndProfitSharingReportTotals,
  setAdditionalExecutivesGrid,
  setBalanceByAge,
  setBalanceByYears,
  setBreakdownByStore,
  setBreakdownByStoreMangement,
  setBreakdownByStoreTotals,
  setBreakdownGrandTotals,
  setCertificates,
  setContributionsByAge,
  setControlSheet,
  setDemographicBadgesNotInPayprofitData,
  setDistributionsAndForfeitures,
  setDistributionsByAge,
  setDuplicateNamesAndBirthdays,
  setDuplicateSSNsData,
  setEligibleEmployees,
  setEmployeeWagesForYear,
  setExecutiveHoursAndDollars,
  setForfeituresAndPoints,
  setForfeituresByAge,
  setGrossWagesReport,
  setNegativeEtvaForSSNsOnPayprofit,
  setProfitMasterApply,
  setProfitMasterRevert,
  setProfitMasterStatus,
  setProfitShareSummaryReport,
  setProfitSharingEdit,
  setProfitSharingLabels,
  setProfitSharingUnder21Report,
  setProfitSharingUpdate,
  setProfitSharingUpdateAdjustmentSummary,
  setRecentlyTerminated,
  setTerminatedLetters,
  setTermination,
  setUnder21BreakdownByStore,
  setUnder21Inactive,
  setUnder21Totals,
  setUnForfeitsDetails,
  setUpdateSummary,
  setVestedAmountsByAge,
  setYearEndProfitSharingReportFrozen,
  setYearEndProfitSharingReportLive,
  setYearEndProfitSharingReportTotals
} from "reduxstore/slices/yearsEndSlice";
import {
  AdhocBeneficiariesReportRequest,
  adhocBeneficiariesReportResponse,
  BadgeNumberRequest,
  BalanceByAge,
  BalanceByYears,
  BreakdownByStoreEmployee,
  BreakdownByStoreRequest,
  BreakdownByStoreTotals,
  CertificateDownloadRequest,
  CertificatePrintRequest,
  CertificatesReportResponse,
  ContributionsByAge,
  ControlSheetRequest,
  ControlSheetResponse,
  DemographicBadgesNotInPayprofitRequestDto,
  DemographicBadgesNotInPayprofitResponse,
  DistributionsAndForfeituresRequestDto,
  DistributionsAndForfeitureTotalsResponse,
  DuplicateNameAndBirthday,
  DuplicateNameAndBirthdayRequestDto,
  DuplicateSSNDetail,
  DuplicateSSNsRequestDto,
  EligibleEmployeeResponseDto,
  EligibleEmployeesRequestDto,
  EmployeeWagesForYear,
  EmployeeWagesForYearRequestDto,
  ExecutiveHoursAndDollars,
  ExecutiveHoursAndDollarsRequestDto,
  ForfeitureAdjustmentDetail,
  ForfeitureAdjustmentUpdateRequest,
  ForfeituresAndPointsResponse,
  ForfeituresByAge,
  FrozenReportsByAgeRequest,
  FrozenReportsForfeituresAndPointsRequest,
  GrandTotalsByStoreResponseDto,
  GrossWagesReportDto,
  GrossWagesReportResponse,
  NegativeEtvaForSSNsOnPayProfit,
  NegativeEtvaForSSNsOnPayprofitRequestDto,
  PagedReportResponse,
  PayBenReportRequest,
  PayBenReportResponse,
  ProfitMasterStatus,
  ProfitShareEditResponse,
  ProfitShareMasterApplyRequest,
  ProfitShareMasterResponse,
  ProfitShareUpdateRequest,
  ProfitShareUpdateResponse,
  ProfitSharingDistributionsByAge,
  ProfitSharingLabel,
  ProfitSharingLabelsRequest,
  ProfitSharingUnder21ReportRequest,
  ProfitSharingUnder21ReportResponse,
  ProfitYearRequest,
  QPAY066BTerminatedWithVestedBalanceRequest,
  QPAY066BTerminatedWithVestedBalanceResponse,
  RecentlyTerminatedResponse,
  StartAndEndDateRequest,
  SuggestedForfeitResponse,
  SuggestForfeitureAdjustmentRequest,
  TerminatedLettersRequest,
  TerminatedLettersResponse,
  TerminationResponse,
  Under21BreakdownByStoreRequest,
  Under21BreakdownByStoreResponse,
  Under21InactiveRequest,
  Under21InactiveResponse,
  Under21TotalsRequest,
  Under21TotalsResponse,
  UnForfeit,
  UpdateSummaryRequest,
  UpdateSummaryResponse,
  VestedAmountsByAge,
  YearEndProfitSharingReportRequest,
  YearEndProfitSharingReportResponse,
  YearEndProfitSharingReportSummaryResponse,
  YearEndProfitSharingReportTotalsResponse
} from "reduxstore/types";
import { Paged } from "smart-ui-library";
import { createDataSourceAwareBaseQuery } from "./api";

// Create intersection type for getUpdateSummary with optional archive
type UpdateSummaryRequestWithArchive = UpdateSummaryRequest & { archive?: boolean };

// Create intersection type for getTerminationReport with optional archive
type TerminationRequestWithArchive = StartAndEndDateRequest & {
  archive?: boolean;
  vestedBalanceValue?: number | null;
  vestedBalanceOperator?: number | null;
};

import {
  clearForfeitureAdjustmentData,
  setForfeitureAdjustmentData
} from "reduxstore/slices/forfeituresAdjustmentSlice";

/* Use the centralized data source aware base query 
   2-minute timeout for year-end reports (after database index optimization) */
const baseQuery = createDataSourceAwareBaseQuery(120000); // 2 minutes in milliseconds
/* ------------------------------------------------------------------------- */

export const YearsEndApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "yearsEndApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0, // Remove data immediately when no longer in use
  refetchOnMountOrArgChange: true, // Always fetch fresh data
  endpoints: (builder) => ({
    updateExecutiveHoursAndDollars: builder.mutation({
      query: ({ ...rest }) => ({
        url: `yearend/executive-hours-and-dollars/`,
        method: "PUT",
        body: rest
      })
      // Note that it seems as if we should do something here to the store
      // after we do the update. Yet the working copy in the grid is
      // the correct data, a refresh is not needed.
    }),
    updateEnrollment: builder.mutation<void, ProfitYearRequest>({
      query: (params) => ({
        url: `yearend/update-enrollment`,
        method: "POST",
        body: params
      })
    }),
    getDuplicateSSNs: builder.query<PagedReportResponse<DuplicateSSNDetail>, DuplicateSSNsRequestDto>({
      query: (params) => ({
        url: `yearend/duplicate-ssns`,
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          profitYear: params.profitYear
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDuplicateSSNsData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getDemographicBadgesNotInPayprofit: builder.query<
      DemographicBadgesNotInPayprofitResponse,
      DemographicBadgesNotInPayprofitRequestDto
    >({
      query: (params) => ({
        url: `yearend/demographic-badges-not-in-payprofit`,
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          profitYear: params.profitYear
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDemographicBadgesNotInPayprofitData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getDistributionsAndForfeitures: builder.query<
      DistributionsAndForfeitureTotalsResponse,
      DistributionsAndForfeituresRequestDto
    >({
      query: (params) => ({
        url: `yearend/distributions-and-forfeitures`,
        method: "POST",
        body: {
          startDate: params.startDate,
          endDate: params.endDate,
          states: params.states,
          taxCodes: params.taxCodes,
          skip: params.pagination.skip,
          take: params.pagination.take,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDistributionsAndForfeitures(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getDuplicateNamesAndBirthdays: builder.query<
      PagedReportResponse<DuplicateNameAndBirthday>,
      DuplicateNameAndBirthdayRequestDto
    >({
      query: (params) => ({
        url: "yearend/duplicate-names-and-birthdays",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          includeFictionalSsnPairs: params.includeFictionalSsnPairs ?? false
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDuplicateNamesAndBirthdays(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    refreshDuplicateNamesAndBirthdaysCache: builder.mutation<string, void>({
      query: () => ({
        url: "yearend/duplicate-names-and-birthdays/refresh-cache",
        method: "POST",
        body: {}
      })
    }),
    getGrossWagesReport: builder.query<GrossWagesReportResponse, GrossWagesReportDto>({
      query: (params) => ({
        url: "yearend/frozen/grosswages",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          minGrossAmount: params.minGrossAmount
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setGrossWagesReport(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getUnForfeits: builder.query<PagedReportResponse<UnForfeit>, StartAndEndDateRequest & { archive?: boolean }>({
      query: (params) => {
        const baseUrl = `yearend/unforfeitures/`;
        const url = params.archive ? `${baseUrl}?archive=true` : baseUrl;

        return {
          url,
          method: "POST",
          body: {
            beginningDate: params.beginningDate,
            endingDate: params.endingDate,
            take: params.pagination.take,
            skip: params.pagination.skip,
            sortBy: params.pagination.sortBy,
            isSortDescending: params.pagination.isSortDescending,
            excludeZeroBalance: params.excludeZeroBalance || false,
            profitYear: params.profitYear
          }
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setUnForfeitsDetails(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getMismatchedSSNsPayprofitAndDemoOnSameBadge: builder.query({
      query: (params) => ({
        url: "yearend/mismatched-ssns-payprofit-and-demo-on-same-badge",
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted({ queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getNegativeEVTASSN: builder.query<
      PagedReportResponse<NegativeEtvaForSSNsOnPayProfit>,
      NegativeEtvaForSSNsOnPayprofitRequestDto
    >({
      query: (params) => ({
        url: "yearend/negative-evta-ssn",
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
          const { data } = await queryFulfilled;
          dispatch(setNegativeEtvaForSSNsOnPayprofit(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getPayprofitBadgeWithoutDemographics: builder.query({
      query: (params) => ({
        url: "yearend/payprofit-badges-without-demographics",
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getEmployeeWagesForYear: builder.query<
      PagedReportResponse<EmployeeWagesForYear>,
      EmployeeWagesForYearRequestDto & { acceptHeader: string }
    >({
      query: (params) => ({
        url: "yearend/wages-current-year",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        },
        headers: {
          Accept: params.acceptHeader
        },
        responseHandler: async (response) => {
          if (params.acceptHeader === "text/csv") {
            return response.blob();
          }
          return response.json();
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setEmployeeWagesForYear(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getAdditionalExecutives: builder.query<
      PagedReportResponse<ExecutiveHoursAndDollars>,
      ExecutiveHoursAndDollarsRequestDto
    >({
      query: (params) => ({
        url: "yearend/executive-hours-and-dollars",
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          profitYear: params.profitYear,
          badgeNumber: params.badgeNumber,
          ssn: params.socialSecurity,
          fullNameContains: params.fullNameContains,
          hasExecutiveHoursAndDollars: params.hasExecutiveHoursAndDollars
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setAdditionalExecutivesGrid(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getExecutiveHoursAndDollars: builder.query<
      PagedReportResponse<ExecutiveHoursAndDollars>,
      ExecutiveHoursAndDollarsRequestDto & { archive?: boolean }
    >({
      query: (params) => ({
        url: `yearend/executive-hours-and-dollars${params.archive ? "?archive=true" : ""}`,
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          profitYear: params.profitYear,
          badgeNumber: params.badgeNumber,
          ssn: params.socialSecurity,
          fullNameContains: params.fullNameContains,
          hasExecutiveHoursAndDollars: params.hasExecutiveHoursAndDollars,
          isMonthlyPayroll: params.isMonthlyPayroll
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setExecutiveHoursAndDollars(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getEligibleEmployees: builder.query<EligibleEmployeeResponseDto, EligibleEmployeesRequestDto>({
      query: (params) => ({
        url: "yearend/eligible-employees",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(_params: EligibleEmployeesRequestDto, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setEligibleEmployees(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getDistributionsByAge: builder.query<ProfitSharingDistributionsByAge, FrozenReportsByAgeRequest>({
      query: (params) => ({
        url: "yearend/frozen/distributions-by-age",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          reportType: params.reportType
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDistributionsByAge(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getContributionsByAge: builder.query<ContributionsByAge, FrozenReportsByAgeRequest>({
      query: (arg) => {
        // Validate profit year range
        if (arg.profitYear < 2020 || arg.profitYear > 2100) {
          console.error("Invalid profit year: Must be between 2020 and 2100");
          // Return a dummy endpoint that won't be called
          return { url: "invalid-request", method: "GET" };
        }

        return {
          url: `yearend/frozen/contributions-by-age`,
          method: "GET",
          params: {
            profitYear: arg.profitYear,
            reportType: arg.reportType
          }
        };
      },
      transformResponse: (response: ContributionsByAge) => {
        return response;
      },
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          // Don't proceed with API call if validation failed
          if (arg.profitYear < 2020 || arg.profitYear > 2100) {
            return;
          }

          const { data } = await queryFulfilled;
          dispatch(setContributionsByAge(data));
        } catch (error) {
          console.error("Error fetching contributions by age:", error);
        }
      }
    }),
    getForfeituresByAge: builder.query<ForfeituresByAge, FrozenReportsByAgeRequest>({
      query: (params) => ({
        url: "yearend/frozen/forfeitures-by-age",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          reportType: params.reportType
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setForfeituresByAge(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getForfeituresAndPoints: builder.query<
      ForfeituresAndPointsResponse,
      FrozenReportsForfeituresAndPointsRequest & {
        suppressAllToastErrors?: boolean;
        onlyNetworkToastErrors?: boolean;
        archive?: boolean;
      }
    >({
      query: (params) => {
        const { suppressAllToastErrors, onlyNetworkToastErrors, archive } = params;
        return {
          url: `yearend/frozen/forfeitures-and-points${archive ? "?archive=true" : ""}`,
          method: "GET",
          params: {
            profitYear: params.profitYear,
            useFrozenData: params.useFrozenData,
            take: params.pagination.take,
            skip: params.pagination.skip,
            sortBy: params.pagination.sortBy,
            isSortDescending: params.pagination.isSortDescending
          },
          meta: { suppressAllToastErrors, onlyNetworkToastErrors }
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setForfeituresAndPoints(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getBalanceByAge: builder.query<BalanceByAge, FrozenReportsByAgeRequest>({
      query: (params) => ({
        url: "yearend/frozen/balance-by-age",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          reportType: params.reportType
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setBalanceByAge(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getBalanceByYears: builder.query<BalanceByYears, FrozenReportsByAgeRequest>({
      query: (params) => ({
        url: "yearend/frozen/balance-by-years",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          reportType: params.reportType
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setBalanceByYears(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getVestingAmountByAge: builder.query<VestedAmountsByAge, ProfitYearRequest & { acceptHeader: string }>({
      query: (params) => ({
        url: "yearend/frozen/vested-amounts-by-age",
        method: "GET",
        params: {
          profitYear: params.profitYear
        },
        headers: {
          Accept: params.acceptHeader
        },
        responseHandler: async (response) => {
          if (params.acceptHeader === "text/csv") {
            return response.blob();
          }
          return response.json();
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setVestedAmountsByAge(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getTerminationReport: builder.query<TerminationResponse, TerminationRequestWithArchive>({
      query: (params) => {
        const body: {
          beginningDate: string;
          endingDate: string;
          skip: number;
          take: number;
          sortBy?: string;
          isSortDescending?: boolean;
          profitYear?: number;
          excludeZeroAndFullyVested?: boolean;
          vestedBalanceValue?: number | null;
          vestedBalanceOperator?: number | null;
        } = {
          beginningDate: params.beginningDate,
          endingDate: params.endingDate,
          skip: params.pagination.skip,
          take: params.pagination.take,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        };

        if (params.profitYear) {
          body.profitYear = params.profitYear;
        }

        if (params.excludeZeroAndFullyVested !== undefined) {
          body.excludeZeroAndFullyVested = params.excludeZeroAndFullyVested;
        }

        if (params.vestedBalanceValue !== null && params.vestedBalanceValue !== undefined) {
          body.vestedBalanceValue = params.vestedBalanceValue;
        }

        if (params.vestedBalanceOperator !== null && params.vestedBalanceOperator !== undefined) {
          body.vestedBalanceOperator = params.vestedBalanceOperator;
        }

        return {
          url: `yearend/terminated-employees${params.archive === true ? "?archive=true" : ""}`,
          method: "POST",
          body
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setTermination(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getRecentlyTerminatedReport: builder.query<RecentlyTerminatedResponse, StartAndEndDateRequest>({
      query: (params) => {
        return {
          url: "yearend/adhoc-terminated-employees-report",
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
          url: "yearend/adhoc-terminated-employees-report-needing-letter",
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
        url: "yearend/adhoc-terminated-employees-report-needing-letter/download",
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
        },
        responseHandler: (response) => response.blob()
      })
    }),
    getProfitShareUpdate: builder.query<ProfitShareUpdateResponse, ProfitShareUpdateRequest>({
      query: (params) => ({
        url: "yearend/profit-sharing-update",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          contributionPercent: params.contributionPercent,
          earningsPercent: params.earningsPercent,
          incomingForfeitPercent: params.incomingForfeitPercent,
          secondaryEarningsPercent: params.secondaryEarningsPercent,
          maxAllowedContributions: params.maxAllowedContributions,
          badgeToAdjust: params.badgeToAdjust,
          adjustContributionAmount: params.adjustContributionAmount,
          adjustEarningsAmount: params.adjustEarningsAmount,
          adjustIncomingForfeitAmount: params.adjustIncomingForfeitAmount,
          badgeToAdjust2: params.badgeToAdjust2,
          adjustEarningsSecondaryAmount: params.adjustEarningsSecondaryAmount,
          take: params.pagination.take,
          skip: params.pagination.skip
          //sortBy: params.pagination.sortBy,
          //isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitSharingUpdate(data));
          dispatch(setProfitSharingUpdateAdjustmentSummary(data.adjustmentsSummary));
          if (arg.badgeToAdjust) {
            //console.log("Added badge: " + arg.badgeToAdjust);
            dispatch(addBadgeNumberToUpdateAdjustmentSummary(arg.badgeToAdjust));
          }
        } catch (err) {
          console.log("Err", err);
          dispatch(clearProfitSharingUpdate());
        }
      }
    }),
    getProfitShareEdit: builder.query<ProfitShareEditResponse, ProfitShareUpdateRequest>({
      query: (params) => ({
        url: "yearend/profit-share-edit",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          contributionPercent: params.contributionPercent,
          earningsPercent: params.earningsPercent,
          incomingForfeitPercent: params.incomingForfeitPercent,
          secondaryEarningsPercent: params.secondaryEarningsPercent,
          maxAllowedContributions: params.maxAllowedContributions,
          badgeToAdjust: params.badgeToAdjust,
          adjustContributionAmount: params.adjustContributionAmount,
          adjustEarningsAmount: params.adjustEarningsAmount,
          adjustIncomingForfeitAmount: params.adjustIncomingForfeitAmount,
          badgeToAdjust2: params.badgeToAdjust2,
          adjustEarningsSecondaryAmount: params.adjustEarningsSecondaryAmount,
          take: params.pagination.take,
          skip: params.pagination.skip
          //sortBy: params.pagination.sortBy,
          //isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitSharingEdit(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitSharingEdit());
        }
      }
    }),
    getProfitMasterStatus: builder.query<ProfitMasterStatus, ProfitYearRequest>({
      query: (params) => ({
        url: "yearend/profit-master-status",
        method: "GET",
        params: {
          profitYear: params.profitYear
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitMasterStatus(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitMasterStatus());
        }
      }
    }),

    getUnder21BreakdownByStore: builder.query<Under21BreakdownByStoreResponse, Under21BreakdownByStoreRequest>({
      query: (params) => ({
        url: "yearend/post-frozen/under-21-breakdown-by-store",
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
          dispatch(clearUnder21BreakdownByStore());
          const { data } = await queryFulfilled;
          dispatch(setUnder21BreakdownByStore(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearUnder21BreakdownByStore());
        }
      }
    }),
    getPostFrozenUnder21: builder.query<ProfitSharingUnder21ReportResponse, ProfitSharingUnder21ReportRequest>({
      query: (params) => ({
        url: "yearend/post-frozen-under-21",
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
          dispatch(clearProfitSharingUnder21Report());
          const { data } = await queryFulfilled;
          dispatch(setProfitSharingUnder21Report(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearUnder21BreakdownByStore());
        }
      }
    }),
    getUnder21Inactive: builder.query<Under21InactiveResponse, Under21InactiveRequest>({
      query: (params) => ({
        url: "yearend/post-frozen/under-21-inactive",
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
          dispatch(clearUnder21Inactive());
          const { data } = await queryFulfilled;
          dispatch(setUnder21Inactive(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearUnder21Inactive());
        }
      }
    }),

    getUnder21Totals: builder.query<Under21TotalsResponse, Under21TotalsRequest>({
      query: (params) => ({
        url: "yearend/post-frozen/totals",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          take: params.pagination.take,
          skip: params.pagination.skip
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(clearUnder21Totals());
          const { data } = await queryFulfilled;
          dispatch(setUnder21Totals(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearUnder21Totals());
        }
      }
    }),

    getMasterApply: builder.mutation<ProfitShareMasterResponse, ProfitShareMasterApplyRequest>({
      query: (params) => ({
        url: "yearend/profit-master-update",
        method: "POST",
        body: params
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitMasterApply(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitMasterApply());
        }
      }
    }),
    getMasterRevert: builder.query<ProfitShareMasterResponse, ProfitYearRequest>({
      query: (params) => ({
        url: "yearend/profit-master-revert",
        method: "GET",
        params: params
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitMasterRevert(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitMasterRevert());
        }
      }
    }),
    getProfitSharingLabels: builder.query<Paged<ProfitSharingLabel>, ProfitSharingLabelsRequest>({
      query: (params) => ({
        url: "yearend/post-frozen/profit-sharing-labels",
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
          const { data } = await queryFulfilled;
          dispatch(setProfitSharingLabels(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitSharingLabels());
        }
      }
    }),
    getYearEndProfitSharingReportLive: builder.query<
      YearEndProfitSharingReportResponse,
      YearEndProfitSharingReportRequest & { archive?: boolean }
    >({
      query: (params) => {
        const { pagination, archive, ...rest } = params;
        return {
          url: `yearend/yearend-profit-sharing-report${archive === true ? "/?archive=true" : ""}`,
          method: "POST",
          body: {
            ...rest,
            useFrozenData: false,
            take: pagination.take,
            skip: pagination.skip,
            sortBy: pagination.sortBy,
            isSortDescending: pagination.isSortDescending
          }
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(clearYearEndProfitSharingReportLive());
          const { data } = await queryFulfilled;
          dispatch(setYearEndProfitSharingReportLive(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearYearEndProfitSharingReportLive());
        }
      }
    }),
    getYearEndProfitSharingReportFrozen: builder.query<
      YearEndProfitSharingReportResponse,
      YearEndProfitSharingReportRequest & { archive?: boolean }
    >({
      query: (params) => {
        const { pagination, archive, ...rest } = params;
        return {
          url: `yearend/yearend-profit-sharing-report${archive === true ? "/?archive=true" : ""}`,
          method: "POST",
          body: {
            ...rest,
            useFrozenData: true,
            take: pagination.take,
            skip: pagination.skip,
            sortBy: pagination.sortBy,
            isSortDescending: pagination.isSortDescending
          }
        };
      },
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(clearYearEndProfitSharingReportFrozen());
          const { data } = await queryFulfilled;
          dispatch(setYearEndProfitSharingReportFrozen(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearYearEndProfitSharingReportFrozen());
        }
      }
    }),
    getYearEndProfitSharingReportTotals: builder.query<
      YearEndProfitSharingReportTotalsResponse,
      BadgeNumberRequest & { archive?: boolean }
    >({
      query: (params) => ({
        url: `yearend/yearend-profit-sharing-report-totals${params.archive === true ? "/?archive=true" : ""}`,
        method: "POST",
        body: {
          ...params
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(clearYearEndProfitSharingReportTotals());
          const { data } = await queryFulfilled;
          dispatch(setYearEndProfitSharingReportTotals(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearYearEndProfitSharingReportTotals());
        }
      }
    }),
    getYearEndProfitSharingSummaryReport: builder.query<
      YearEndProfitSharingReportSummaryResponse,
      BadgeNumberRequest & { archive?: boolean }
    >({
      query: (params) => ({
        url: `yearend/yearend-profit-sharing-summary-report${params.archive === true ? "?archive=true" : ""}`,
        method: "POST",
        body: {
          useFrozenData: params.useFrozenData,
          profitYear: params.profitYear,
          badgeNumber: params.badgeNumber,
          skip: 0,
          take: 255
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitShareSummaryReport(data));
        } catch (err) {
          console.error(
            "Error fetching year-end profit sharing summary report:",
            err instanceof Error ? err.stack || err.message : JSON.stringify(err, null, 2)
          );
        }
      }
    }),
    getUpdateSummary: builder.query<UpdateSummaryResponse, UpdateSummaryRequestWithArchive>({
      query: (params) => ({
        url: `yearend/frozen/updatesummary`,
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          ...(params.archive && { archive: params.archive })
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setUpdateSummary(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getControlSheet: builder.query<ControlSheetResponse, ControlSheetRequest>({
      query: (params) => ({
        url: `yearend/post-frozen/control-sheet`,
        method: "GET",
        params: {
          profitYear: params.profitYear,
          skip: params.pagination.skip,
          take: params.pagination.take,
          isSortDescending: params.pagination.isSortDescending,
          sortBy: params.pagination.sortBy
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setControlSheet(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getForfeitureAdjustments: builder.query<
      SuggestedForfeitResponse,
      SuggestForfeitureAdjustmentRequest & { onlyNetworkToastErrors?: boolean }
    >({
      query: (params) => {
        const { onlyNetworkToastErrors, ...requestParams } = params;
        return {
          url: "yearend/forfeiture-adjustments",
          method: "GET",
          params: {
            ssn: requestParams.ssn,
            badge: requestParams.badge,
            profitYear: requestParams.profitYear,
            skip: requestParams.skip || 0,
            take: requestParams.take || 255,
            sortBy: requestParams.sortBy,
            isSortDescending: requestParams.isSortDescending
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
          url: "yearend/forfeiture-adjustments/update",
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
          url: "yearend/forfeiture-adjustments/bulk-update",
          method: "PUT",
          body: params
        })
      }
    ),

    finalizeReport: builder.mutation<void, { profitYear: number }>({
      query: (params) => ({
        url: "yearend/final",
        method: "POST",
        body: params
      })
    }),
    adhocBeneficiariesReport: builder.query<adhocBeneficiariesReportResponse, AdhocBeneficiariesReportRequest>({
      query: (params) => ({
        url: "yearend/adhoc-beneficiaries-report",
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
    payBenReport: builder.query<PayBenReportResponse, PayBenReportRequest>({
      query: (params) => ({
        url: "yearend/payben-report",
        method: "GET",
        params: {
          id: params.id,
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
    getCertificatesReport: builder.query<CertificatesReportResponse, CertificatePrintRequest>({
      query: (params) => ({
        url: "yearend/post-frozen/certificates",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          badgeNumbers: params.badgeNumbers,
          ssns: params.ssns,
          skip: params.skip,
          take: params.take,
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setCertificates(data));
        } catch (err) {
          console.log("Err", err);
          dispatch(clearCertificates());
        }
      }
    }),
    downloadCertificatesFile: builder.query<Blob, CertificateDownloadRequest>({
      query: (params) => ({
        url: "yearend/post-frozen/certificates/download",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          badgeNumbers: params.badgeNumbers,
          ssns: params.ssns
        },
        responseHandler: (response) => response.blob()
      })
    }),
    // **************************
    // Breakdown By Store Queries
    //.  with PagedReportResponse<BreakdownByStoreEmployee>
    // **************************
    // 1. Get Breakdown By Store

    getBreakdownByStore: builder.query<PagedReportResponse<BreakdownByStoreEmployee>, BreakdownByStoreRequest>({
      query: (params) => ({
        url: "yearend/breakdown-by-store",
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
            dispatch(setBreakdownByStoreMangement(data));
          } else {
            dispatch(setBreakdownByStore(data));
          }
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),

    getBreakdownByStoreTotals: builder.query<BreakdownByStoreTotals, BreakdownByStoreRequest>({
      query: (params) => ({
        url: `yearend/breakdown-by-store/${params.storeNumber}/totals`,
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
        url: `yearend/breakdown-by-store/totals`,
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
        url: "yearend/breakdown-by-store/terminated/withcurrentbalance/notvested",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          skip: params.pagination.skip,
          take: params.pagination.take,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      })
    })
  })
});

export const {
  useLazyGetAdditionalExecutivesQuery,
  useLazyGetBalanceByAgeQuery,
  useLazyGetBalanceByYearsQuery,
  useLazyGetBreakdownByStoreQuery,
  useLazyGetBreakdownByStoreTotalsQuery,
  useLazyGetCertificatesReportQuery,
  useLazyGetContributionsByAgeQuery,
  useLazyGetControlSheetQuery,
  useLazyGetDemographicBadgesNotInPayprofitQuery,
  useLazyGetDistributionsAndForfeituresQuery,
  useLazyGetDistributionsByAgeQuery,
  useLazyDownloadCertificatesFileQuery,
  useLazyGetDuplicateNamesAndBirthdaysQuery,
  useRefreshDuplicateNamesAndBirthdaysCacheMutation,
  useLazyGetDuplicateSSNsQuery,
  useLazyGetEligibleEmployeesQuery,
  //useLazyGetEmployeesOnMilitaryLeaveQuery,
  useLazyGetEmployeeWagesForYearQuery,
  useLazyGetExecutiveHoursAndDollarsQuery,
  useLazyGetForfeituresAndPointsQuery,
  useLazyGetForfeituresByAgeQuery,
  useLazyGetGrossWagesReportQuery,
  useLazyGetUnForfeitsQuery,
  useLazyGetNegativeEVTASSNQuery,
  useLazyGetProfitShareEditQuery,
  useLazyGetProfitShareUpdateQuery,
  useLazyGetTerminationReportQuery,
  useLazyGetUnder21BreakdownByStoreQuery,
  useLazyGetUnder21InactiveQuery,
  useLazyGetUnder21TotalsQuery,
  useLazyGetVestingAmountByAgeQuery,
  useLazyGetPostFrozenUnder21Query,
  useLazyGetYearEndProfitSharingReportLiveQuery,
  useLazyGetYearEndProfitSharingReportFrozenQuery,
  useLazyGetYearEndProfitSharingReportTotalsQuery,
  useUpdateExecutiveHoursAndDollarsMutation,
  useLazyGetYearEndProfitSharingSummaryReportQuery,
  useGetMasterApplyMutation,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitSharingLabelsQuery,
  useLazyGetProfitMasterStatusQuery,
  useLazyGetBreakdownGrandTotalsQuery,
  useLazyGetForfeitureAdjustmentsQuery,
  useUpdateForfeitureAdjustmentMutation,
  useUpdateForfeitureAdjustmentBulkMutation,
  useLazyGetUpdateSummaryQuery,
  useUpdateEnrollmentMutation,
  useFinalizeReportMutation,
  useLazyAdhocBeneficiariesReportQuery,
  useLazyPayBenReportQuery,
  useLazyGetQPAY066BTerminatedWithVestedBalanceQuery,
  useLazyGetRecentlyTerminatedReportQuery,
  useLazyGetTerminatedLettersReportQuery,
  useLazyGetTerminatedLettersDownloadQuery
} = YearsEndApi;
