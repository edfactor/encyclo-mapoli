import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import {
  addBadgeNumberToUpdateAdjustmentSummary,
  clearBreakdownByStore, clearBreakdownByStoreTotals,
  clearProfitMasterApply,
  clearProfitMasterRevert,
  clearProfitMasterStatus,
  clearProfitSharingEdit,
  clearProfitSharingLabels,
  clearProfitSharingUpdate,
  clearUnder21BreakdownByStore,
  clearUnder21Inactive,
  clearUnder21Totals,
  clearYearEndProfitSharingReport,
  setAdditionalExecutivesGrid,
  setBalanceByAge,
  setBalanceByYears,
  setBreakdownByStore, setBreakdownByStoreTotals,
  setContributionsByAge,
  setDemographicBadgesNotInPayprofitData,
  setDistributionsAndForfeitures,
  setDistributionsByAge,
  setDuplicateNamesAndBirthdays,
  setDuplicateSSNsData,
  setEligibleEmployees,
  setEmployeesOnMilitaryLeaveDetails,
  setEmployeeWagesForYear,
  setExecutiveHoursAndDollars,
  setForfeituresAndPoints,
  setForfeituresByAge,
  setGrossWagesReport,
  setMilitaryAndRehireForfeituresDetails,
  setMissingCommaInPYName,
  setNegativeEtvaForSSNsOnPayprofit,
  setProfitMasterApply,
  setProfitMasterRevert,
  setProfitMasterStatus,
  setProfitShareSummaryReport,
  setProfitSharingEdit,
  setProfitSharingLabels,
  setProfitSharingUpdate,
  setProfitSharingUpdateAdjustmentSummary,
  setTermination,
  setUnder21BreakdownByStore,
  setUnder21Inactive,
  setUnder21Totals,
  setUpdateSummary,
  setVestedAmountsByAge,
  setYearEndProfitSharingReport
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import {
  BalanceByAge,
  BalanceByYears,
  BreakdownByStoreRequest,
  BreakdownByStoreResponse,
  ContributionsByAge,
  DemographicBadgesNotInPayprofitRequestDto,
  DemographicBadgesNotInPayprofitResponse,
  DistributionsAndForfeitures,
  DistributionsAndForfeituresRequestDto,
  DuplicateNameAndBirthday,
  DuplicateNameAndBirthdayRequestDto,
  DuplicateSSNDetail,
  DuplicateSSNsRequestDto,
  EligibleEmployeeResponseDto,
  EligibleEmployeesRequestDto,
  EmployeesOnMilitaryLeaveRequestDto,
  EmployeesOnMilitaryLeaveResponse,
  EmployeeWagesForYear,
  EmployeeWagesForYearRequestDto,
  ExecutiveHoursAndDollars,
  ExecutiveHoursAndDollarsRequestDto,
  ForfeituresAndPoints,
  ForfeituresByAge,
  FrozenProfitYearRequest,
  FrozenReportsByAgeRequest,
  FrozenReportsForfeituresAndPointsRequest,
  GrossWagesReportDto,
  GrossWagesReportResponse,
  MilitaryAndRehireForfeiture,
  MissingCommasInPYName,
  MissingCommasInPYNameRequestDto,
  NegativeEtvaForSSNsOnPayProfit,
  NegativeEtvaForSSNsOnPayprofitRequestDto,
  PagedReportResponse,
  ProfitMasterStatus,
  ProfitShareEditResponse,
  ProfitShareMasterApplyRequest,
  ProfitShareMasterResponse,
  ProfitShareUpdateRequest,
  ProfitShareUpdateResponse,
  ProfitSharingDistributionsByAge,
  ProfitSharingLabel,
  ProfitSharingLabelsRequest,
  ProfitYearRequest,
  RehireForfeituresRequest,
  TerminationRequest,
  TerminationResponse,
  Under21BreakdownByStoreRequest,
  Under21BreakdownByStoreResponse,
  Under21InactiveRequest,
  Under21InactiveResponse,
  Under21TotalsRequest,
  Under21TotalsResponse,
  UpdateSummaryRequest,
  UpdateSummaryResponse,
  VestedAmountsByAge,
  YearEndProfitSharingReportRequest,
  YearEndProfitSharingReportResponse,
  YearEndProfitSharingReportSummaryResponse,
  ForfeitureAdjustmentRequest,
  ForfeitureAdjustmentResponse,
  ForfeitureAdjustmentUpdateRequest,
  ForfeitureAdjustmentDetail, BreakdownByStoreTotals
} from "reduxstore/types";
import { tryddmmyyyyToDate } from "../../utils/dateUtils";
import { Paged } from "smart-ui-library";
import { url } from "./api";

import {
  setForfeitureAdjustmentData,
  clearForfeitureAdjustmentData
} from "reduxstore/slices/forfeituresAdjustmentSlice";

export const YearsEndApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/`,
    prepareHeaders: (headers, { getState }) => {
      const root = getState() as RootState;
      const token = root.security.token;
      const impersonating = root.security.impersonating;
      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }
      if (impersonating) {
        headers.set("impersonation", impersonating);
      } else {
        const localImpersonation = localStorage.getItem("impersonatingRole");
        if (localImpersonation) {
          headers.set("impersonation", localImpersonation);
        }
      }
      return headers;
    }
  }),
  reducerPath: "yearsEndApi",
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
    getDuplicateSSNs: builder.query<PagedReportResponse<DuplicateSSNDetail>, DuplicateSSNsRequestDto>({
      query: (params) => ({
        url: `yearend/duplicate-ssns`,
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDemographicBadgesNotInPayprofitData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getDistributionsAndForfeitures: builder.query<
      PagedReportResponse<DistributionsAndForfeitures>,
      DistributionsAndForfeituresRequestDto
    >({
      query: (params) => ({
        url: `yearend/distributions-and-forfeitures`,
        method: "GET",
        params: {
          profitYear: params.profitYear,
          includeOutgoingForfeitures: params.includeOutgoingForfeitures,
          startMonth: params.startMonth,
          endMonth: params.endMonth,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setDuplicateNamesAndBirthdays(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getEmployeesOnMilitaryLeave: builder.query<
      PagedReportResponse<EmployeesOnMilitaryLeaveResponse>,
      EmployeesOnMilitaryLeaveRequestDto
    >({
      query: (params) => ({
        url: "yearend/employees-on-military-leave",
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setEmployeesOnMilitaryLeaveDetails(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setGrossWagesReport(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getRehireForfeitures: builder.query<PagedReportResponse<MilitaryAndRehireForfeiture>, RehireForfeituresRequest>({
      query: (params) => ({
        url: `yearend/rehire-forfeitures/`,
        method: "POST",
        body: {
          profitYear: params.profitYear,
          beginningDate: params.beginningDate ? tryddmmyyyyToDate(params.beginningDate) : params.beginningDate,
          endingDate: params.endingDate ? tryddmmyyyyToDate(params.endingDate) : params.endingDate,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMilitaryAndRehireForfeituresDetails(data));
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
          const { data } = await queryFulfilled;
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getNamesMissingCommas: builder.query<PagedReportResponse<MissingCommasInPYName>, MissingCommasInPYNameRequestDto>({
      query: (params) => ({
        url: "yearend/names-missing-commas",
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMissingCommaInPYName(data));
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted({ dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
          hasExecutiveHoursAndDollars: params.hasExecutiveHoursAndDollars,
          isMonthlyPayroll: params.isMonthlyPayroll
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(params: EligibleEmployeesRequestDto, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setForfeituresByAge(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getForfeituresAndPoints: builder.query<ForfeituresAndPoints, FrozenReportsForfeituresAndPointsRequest>({
      query: (params) => ({
        url: "yearend/frozen/forfeitures-and-points",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          useFrozenData: params.useFrozenData,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setVestedAmountsByAge(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getTerminationReport: builder.query<TerminationResponse, TerminationRequest>({
      query: (params) => {
        // Validate profit year range
        if (params.profitYear < 2020 || params.profitYear > 2100) {
          console.error("Invalid profit year: Must be between 2020 and 2100");
          // Return a dummy endpoint that won't be called
          return { url: "invalid-request", method: "GET" };
        }

        return {
          url: "yearend/terminated-employee-and-beneficiary",
          method: "GET",
          params: {
            profitYear: params.profitYear,
            skip: params.pagination.skip,
            take: params.pagination.take,
            sortBy: params.pagination.sortBy,
            isSortDescending: params.pagination.isSortDescending
          }
        };
      },
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          // Don't proceed with API call if validation failed
          if (arg.profitYear < 2020 || arg.profitYear > 2100) {
            return;
          }

          const { data } = await queryFulfilled;
          dispatch(setTermination(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitMasterStatus(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitMasterStatus());
        }
      }
    }),
    getBreakdownByStore: builder.query<BreakdownByStoreResponse, BreakdownByStoreRequest>({
      query: (params) => ({
        url: "yearend/breakdown-by-store",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          storeNumber: params.storeNumber,
          storeManagement: params.storeManagement,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          dispatch(clearBreakdownByStore());
          const { data } = await queryFulfilled;
          dispatch(setBreakdownByStore(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearBreakdownByStore());
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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

    getMasterApply: builder.query<ProfitShareMasterResponse, ProfitShareMasterApplyRequest>({
      query: (params) => ({
        url: "yearend/profit-master-update",
        method: "GET",
        params: params
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitSharingLabels(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitSharingLabels());
        }
      }
    }),
    getYearEndProfitSharingReport: builder.query<YearEndProfitSharingReportResponse, YearEndProfitSharingReportRequest>(
      {
        query: (params) => ({
          url: "yearend/yearend-profit-sharing-report",
          method: "POST",
          body: {
            ...params,
            take: params.pagination.take,
            skip: params.pagination.skip,
            sortBy: params.pagination.sortBy,
            isSortDescending: params.pagination.isSortDescending
          }
        }),
        async onQueryStarted(arg, { dispatch, queryFulfilled }) {
          try {
            dispatch(clearYearEndProfitSharingReport());
            const { data } = await queryFulfilled;
            dispatch(setYearEndProfitSharingReport(data));
          } catch (err) {
            console.log("Err: " + err);
            dispatch(clearYearEndProfitSharingReport());
          }
        }
      }
    ),
    getYearEndProfitSharingSummaryReport: builder.query<
      YearEndProfitSharingReportSummaryResponse,
      FrozenProfitYearRequest
    >({
      query: (params) => ({
        url: "yearend/yearend-profit-sharing-summary-report",
        method: "GET",
        params: {
          useFrozenData: params.useFrozenData,
          profitYear: params.profitYear,
          skip: 0,
          take: 255
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitShareSummaryReport(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getUpdateSummary: builder.query<UpdateSummaryResponse, UpdateSummaryRequest>({
      query: (params) => ({
        url: `yearend/frozen/updatesummary`,
        method: "GET",
        params: {
          profitYear: params.profitYear,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setUpdateSummary(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getForfeitureAdjustments: builder.query<ForfeitureAdjustmentResponse, ForfeitureAdjustmentRequest>({
      query: (params) => ({
        url: "yearend/forfeiture-adjustments",
        method: "GET",
        params: {
          ssn: params.ssn,
          badge: params.badge,
          profitYear: params.profitYear,
          skip: params.skip || 0,
          take: params.take || 255,
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setForfeitureAdjustmentData(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearForfeitureAdjustmentData());
        }
      }
    }),
    updateForfeitureAdjustment: builder.mutation<ForfeitureAdjustmentDetail, ForfeitureAdjustmentUpdateRequest>({
      query: (params) => ({
        url: "yearend/forfeiture-adjustments/update",
        method: "PUT",
        body: params
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
  useLazyGetContributionsByAgeQuery,
  useLazyGetDemographicBadgesNotInPayprofitQuery,
  useLazyGetDistributionsAndForfeituresQuery,
  useLazyGetDistributionsByAgeQuery,
  useLazyGetDuplicateNamesAndBirthdaysQuery,
  useLazyGetDuplicateSSNsQuery,
  useLazyGetEligibleEmployeesQuery,
  useLazyGetEmployeesOnMilitaryLeaveQuery,
  useLazyGetEmployeeWagesForYearQuery,
  useLazyGetExecutiveHoursAndDollarsQuery,
  useLazyGetForfeituresAndPointsQuery,
  useLazyGetForfeituresByAgeQuery,
  useLazyGetGrossWagesReportQuery,
  useLazyGetRehireForfeituresQuery,
  useLazyGetNamesMissingCommasQuery,
  useLazyGetNegativeEVTASSNQuery,
  useLazyGetProfitShareEditQuery,
  useLazyGetProfitShareUpdateQuery,
  useLazyGetTerminationReportQuery,
  useLazyGetUnder21BreakdownByStoreQuery,
  useLazyGetUnder21InactiveQuery,
  useLazyGetUnder21TotalsQuery,
  useLazyGetVestingAmountByAgeQuery,
  useLazyGetYearEndProfitSharingReportQuery,
  useUpdateExecutiveHoursAndDollarsMutation,
  useLazyGetYearEndProfitSharingSummaryReportQuery,
  useLazyGetMasterApplyQuery,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitSharingLabelsQuery,
  useLazyGetProfitMasterStatusQuery,
  useGetForfeitureAdjustmentsQuery,
  useLazyGetForfeitureAdjustmentsQuery,
  useUpdateForfeitureAdjustmentMutation
} = YearsEndApi;
