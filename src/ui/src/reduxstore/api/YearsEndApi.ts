import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import {
  clearProfitEdit,
  clearProfitUpdate,
  clearYearEndProfitSharingReport,
  setAdditionalExecutivesGrid,
  setBalanceByAge,
  setBalanceByYears,
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
  setMasterInquiryData,
  setMilitaryAndRehireForfeituresDetails,
  setMilitaryAndRehireProfitSummaryDetails,
  setMissingCommaInPYName,
  setNegativeEtvaForSSNsOnPayprofit,
  setProfitEdit,
  setProfitMasterApply,
  setProfitMasterRevert,
  setProfitUpdate,
  setTermination,
  setVestedAmountByAge,
  setYearEndProfitSharingReport
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import {
  BalanceByAge,
  BalanceByYears,
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
  FrozenReportsByAgeRequest,
  FrozenReportsForfeituresAndPointsRequest,
  MasterInquiryRequest,
  MasterInquiryResponseType,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireForfeituresRequestDto,
  MilitaryAndRehireProfitSummary,
  MilitaryAndRehireProfitSummaryRequestDto,
  MissingCommasInPYName,
  MissingCommasInPYNameRequestDto,
  NegativeEtvaForSSNsOnPayProfit,
  NegativeEtvaForSSNsOnPayprofitRequestDto,
  PagedReportResponse,
  ProfitShareEditResponse,
  ProfitShareMasterResponse,
  ProfitShareUpdateRequest,
  ProfitShareUpdateResponse,
  ProfitSharingDistributionsByAge,
  ProfitYearRequest,
  TerminationRequest,
  TerminationResponse,
  VestedAmountsByAge,
  YearEndProfitSharingEmployee,
  YearEndProfitSharingReportRequest
} from "reduxstore/types";
import { url } from "./api";

export const YearsEndApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/`,
    prepareHeaders: (headers, { getState }) => {
      const token = (getState() as RootState).security.token;
      const impersonating = (getState() as RootState).security.impersonating;
      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }
      if (impersonating) {
        headers.set("impersonation", impersonating);
      } else {
        const localImpersonation = localStorage.getItem("impersonatingRole");
        !!localImpersonation && headers.set("impersonation", localImpersonation);
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
    getMilitaryAndRehireForfeitures: builder.query<
      PagedReportResponse<MilitaryAndRehireForfeiture>,
      MilitaryAndRehireForfeituresRequestDto
    >({
      query: (params) => ({
        url: `yearend/military-and-rehire-forfeitures/${params.reportingYear}`,
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
          dispatch(setMilitaryAndRehireForfeituresDetails(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getMilitaryAndRehireProfitSummary: builder.query<
      PagedReportResponse<MilitaryAndRehireProfitSummary>,
      MilitaryAndRehireProfitSummaryRequestDto
    >({
      query: (params) => ({
        url: `yearend/military-and-rehire-profit-summary/${params.reportingYear}`,
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
          dispatch(setMilitaryAndRehireProfitSummaryDetails(data));
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
          skip: params.pagination.skip
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
      query: (params) => ({
        url: "yearend/frozen/contributions-by-age",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          reportType: params.reportType
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setContributionsByAge(data));
        } catch (err) {
          console.log("Err: " + err);
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
          useFrozenData: params.useFrozenData
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
    getProfitMasterInquiry: builder.query<MasterInquiryResponseType, MasterInquiryRequest>({
      query: (params) => ({
        url: "master/master-inquiry",
        method: "GET",
        params: {
          badgeNumber: params.badgeNumber,
          startProfitYear: params.startProfitYear,
          endProfitYear: params.endProfitYear,
          startProfitMonth: params.startProfitMonth,
          endProfitMonth: params.endProfitMonth,
          profitCode: params.profitCode,
          contributionAmount: params.contributionAmount,
          earningsAmount: params.earningsAmount,
          forfeitureAmount: params.forfeitureAmount,
          paymentAmount: params.paymentAmount,
          socialSecurity: params.socialSecurity,
          paymentType: params.paymentType,
          memberType: params.memberType,
          comment: params.comment,
          take: params.pagination.take,
          skip: params.pagination.skip
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMasterInquiryData(data));
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
          dispatch(setVestedAmountByAge(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getTerminationReport: builder.query<TerminationResponse, TerminationRequest>({
      query: (params) => ({
        url: "yearend/terminated-employee-and-beneficiary",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          skip: params.pagination.skip,
          take: params.pagination.take
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
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
        params: params
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitUpdate(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitUpdate());
        }
      }
    }),
    getProfitShareEdit: builder.query<ProfitShareEditResponse, ProfitShareUpdateRequest>({
      query: (params) => ({
        url: "yearend/profit-share-edit",
        method: "GET",
        params: params
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitEdit(data));
        } catch (err) {
          console.log("Err: " + err);
          dispatch(clearProfitEdit());
        }
      }
    }),
    getMasterApply: builder.query<ProfitShareMasterResponse, ProfitShareUpdateRequest>({
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
          dispatch(clearProfitEdit());
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
          dispatch(clearProfitEdit());
        }
      }
    }),
    getYearEndProfitSharingReport: builder.query<
      PagedReportResponse<YearEndProfitSharingEmployee>,
      YearEndProfitSharingReportRequest
    >({
      query: (params) => ({
        url: "yearend/yearend-profit-sharing-report",
        method: "GET",
        params: {
          ...params,
          take: params.pagination.take,
          skip: params.pagination.skip
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
    })
  })
});

export const {
  useLazyGetAdditionalExecutivesQuery,
  useLazyGetBalanceByAgeQuery,
  useLazyGetBalanceByYearsQuery,
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
  useLazyGetMasterApplyQuery,
  useLazyGetMasterRevertQuery,
  useLazyGetMilitaryAndRehireForfeituresQuery,
  useLazyGetMilitaryAndRehireProfitSummaryQuery,
  useLazyGetNamesMissingCommasQuery,
  useLazyGetNegativeEVTASSNQuery,
  useLazyGetProfitMasterInquiryQuery,
  useLazyGetProfitShareEditQuery,
  useLazyGetProfitShareUpdateQuery,
  useLazyGetTerminationReportQuery,
  useLazyGetVestingAmountByAgeQuery,
  useLazyGetYearEndProfitSharingReportQuery,
  useUpdateExecutiveHoursAndDollarsMutation
} = YearsEndApi;
