import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
  DemographicBadgesNotInPayprofitRequestDto,
  DemographicBadgesNotInPayprofitResponse,
  DistributionsAndForfeitures,
  DistributionsAndForfeituresRequestDto,
  FrozenReportsByAgeRequest,
  DuplicateNameAndBirthday,
  DuplicateNameAndBirthdayRequestDto,
  DuplicateSSNDetail,
  DuplicateSSNsRequestDto,
  EligibleEmployeeResponseDto,
  EligibleEmployeesRequestDto,
  ExecutiveHoursAndDollars,
  ExecutiveHoursAndDollarsRequestDto,
  MasterInquiryDetail,
  MasterInquryRequest,
  MilitaryAndRehire,
  MilitaryAndRehireForfeiture,
  MilitaryAndRehireForfeituresRequestDto,
  MilitaryAndRehireProfitSummary,
  MilitaryAndRehireProfitSummaryRequestDto,
  MilitaryAndRehireRequestDto,
  MissingCommasInPYName,
  MissingCommasInPYNameRequestDto,
  NegativeEtvaForSSNsOnPayProfit,
  NegativeEtvaForSSNsOnPayprofitRequestDto,
  PagedReportResponse,
  ProfitSharingDistributionsByAge, ContributionsByAge
} from "reduxstore/types";
import {
  setDemographicBadgesNotInPayprofitData,
  setDistributionsAndForfeitures,
  setDistributionsByAge,
  setContributionsByAge,
  setDuplicateNamesAndBirthdays,
  setDuplicateSSNsData,
  setEligibleEmployees,
  setExecutiveHoursAndDollars,
  setMasterInquiryData,
  setMilitaryAndRehireDetails,
  setMilitaryAndRehireForfeituresDetails,
  setMilitaryAndRehireProfitSummaryDetails,
  setMissingCommaInPYName,
  setNegativeEtvaForSssnsOnPayprofit
} from "reduxstore/slices/yearsEndSlice";
import { url } from "./api";
import { Paged } from "smart-ui-library";

export const YearsEndApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/`,
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
          impersonation: params.impersonation
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
          impersonation: params.impersonation
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
    getMilitaryAndRehire: builder.query<PagedReportResponse<MilitaryAndRehire>, MilitaryAndRehireRequestDto>({
      query: (params) => ({
        url: "yearend/military-and-rehire",
        method: "GET",
        params: {
          take: params.pagination.take,
          skip: params.pagination.skip
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMilitaryAndRehireDetails(data));
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
          dispatch(setNegativeEtvaForSssnsOnPayprofit(data));
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
    getWagesCurrentYear: builder.query({
      query: (params) => ({
        url: "yearend/wages-current-year",
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
    getWagesPreviousYear: builder.query({
      query: (params) => ({
        url: "yearend/wages-previous-year",
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
          fullNameContains: params.fullNameContains,
          hasExecutiveHoursAndDollars: params.hasExecutiveHoursAndDollars
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
    getProfitMasterInquiry: builder.query<Paged<MasterInquiryDetail>, MasterInquryRequest>({
      query: (params) => ({
        url: "yearend/master-inquiry",
        method: "GET",
        params: {
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
  useLazyGetWagesPreviousYearQuery,
  useLazyGetDistributionsAndForfeituresQuery,
  useLazyGetExecutiveHoursAndDollarsQuery,
  useLazyGetEligibleEmployeesQuery,
  useLazyGetDistributionsByAgeQuery,
  useLazyGetContributionsByAgeQuery,
  useLazyGetProfitMasterInquiryQuery
} = YearsEndApi;
