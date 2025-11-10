import { createApi } from "@reduxjs/toolkit/query/react";
import { setMasterInquiryGroupingData, setMasterInquiryResults } from "reduxstore/slices/inquirySlice";
import { Paged } from "smart-ui-library";
import {
  EmployeeDetails,
  GroupedProfitSummaryDto,
  MasterInquiryMemberRequest,
  MasterInquiryRequest,
  MasterInquiryResponseDto
} from "../types";
import { createDataSourceAwareBaseQuery } from "./api";

// In-flight request tracking to prevent duplicate API calls
const inFlightRequests = new Map<string, Promise<unknown>>();

const baseQuery = createDataSourceAwareBaseQuery();
export const InquiryApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "inquiryApi",
  tagTypes: ["MemberDetails", "ProfitDetails"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    // Master Inquiry API endpoints
    searchProfitMasterInquiry: builder.query<Paged<EmployeeDetails>, MasterInquiryRequest>({
      query: (params) => ({
        url: "master/master-inquiry/search",
        method: "POST",
        body: {
          badgeNumber: params.badgeNumber,
          psnSuffix: params.psnSuffix,
          profitYear: params.profitYear,
          endProfitYear: params.endProfitYear,
          startProfitMonth: params.startProfitMonth,
          endProfitMonth: params.endProfitMonth,
          profitCode: params.profitCode,
          contributionAmount: params.contributionAmount,
          earningsAmount: params.earningsAmount,
          forfeitureAmount: params.forfeitureAmount,
          paymentAmount: params.paymentAmount,
          ssn: params.ssn,
          paymentType: params.paymentType,
          memberType: params.memberType,
          name: params.name,
          take: params.pagination.take,
          skip: params.pagination.skip,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          _timestamp: params._timestamp
        }
      }),
      async onQueryStarted(args, { queryFulfilled }) {
        const requestKey = JSON.stringify({
          badge: args.badgeNumber,
          ssn: args.ssn,
          name: args.name,
          profitYear: args.profitYear,
          endProfitYear: args.endProfitYear,
          startProfitMonth: args.startProfitMonth,
          endProfitMonth: args.endProfitMonth,
          memberType: args.memberType,
          paymentType: args.paymentType
        });

        if (inFlightRequests.has(requestKey)) {
          console.log("[InquiryApi] Skipping duplicate search request");
          return;
        }

        inFlightRequests.set(requestKey, queryFulfilled);
        try {
          await queryFulfilled;
        } finally {
          inFlightRequests.delete(requestKey);
        }
      }
    }),
    getProfitMasterInquiryMember: builder.query<EmployeeDetails, MasterInquiryMemberRequest>({
      query: (params) => ({
        url: "master/master-inquiry/member",
        method: "POST",
        body: params
      }),
      providesTags: (_result, _error, args) => [
        { type: "MemberDetails" as const, id: `${args.memberType}-${args.id}` }
      ],
      async onQueryStarted(args, { queryFulfilled }) {
        const requestKey = `member-${args.memberType}-${args.id}`;

        if (inFlightRequests.has(requestKey)) {
          console.log("[InquiryApi] Skipping duplicate member details request");
          return;
        }

        inFlightRequests.set(requestKey, queryFulfilled);
        try {
          await queryFulfilled;
        } finally {
          inFlightRequests.delete(requestKey);
        }
      }
    }),
    getProfitMasterInquiryMemberDetails: builder.query<
      Paged<MasterInquiryResponseDto>,
      { memberType: number; id: number; skip?: number; take?: number; sortBy?: string; isSortDescending?: boolean }
    >({
      query: ({ memberType, id, ...pagination }) => ({
        url: `master/master-inquiry/member/${memberType}/${id}/details`,
        method: "GET",
        params: pagination
      }),
      providesTags: (_result, _error, args) => [
        { type: "ProfitDetails" as const, id: `${args.memberType}-${args.id}` }
      ],
      async onQueryStarted(args, { dispatch, queryFulfilled }) {
        const requestKey = `profit-${args.memberType}-${args.id}`;

        if (inFlightRequests.has(requestKey)) {
          console.log("[InquiryApi] Skipping duplicate profit details request");
          return;
        }

        inFlightRequests.set(requestKey, queryFulfilled);
        try {
          const { data } = await queryFulfilled;
          const { results } = data;
          const transformedResults = results.map((item: MasterInquiryResponseDto) => ({
            ...item,
            transactionDate: item.transactionDate ? new Date(item.transactionDate) : undefined
          }));
          dispatch(setMasterInquiryResults(transformedResults));
        } catch (err) {
          console.error("Failed to fetch profit master inquiry member details:", err);
        } finally {
          inFlightRequests.delete(requestKey);
        }
      }
    }),
    getProfitMasterInquiryFilteredDetails: builder.query<
      { results: MasterInquiryResponseDto[]; total: number },
      {
        memberType: number;
        id?: number;
        profitYear?: number;
        monthToDate?: number;
        badgeNumber?: number;
        psnSuffix?: number;
        ssn?: string;
        endProfitYear?: number;
        startProfitMonth?: number;
        endProfitMonth?: number;
        profitCode?: number;
        contributionAmount?: number;
        earningsAmount?: number;
        forfeitureAmount?: number;
        paymentAmount?: number;
        name?: string;
        paymentType?: number;
        skip?: number;
        take?: number;
        sortBy?: string;
        isSortDescending?: boolean;
      }
    >({
      query: ({ memberType, ...pagination }) => ({
        url: `master/master-inquiry/member/details`,
        method: "POST",
        body: {
          memberType,
          ...pagination
        }
      })
    }),
    getProfitMasterInquiryGrouping: builder.query<Paged<GroupedProfitSummaryDto>, MasterInquiryRequest>({
      query: (params) => ({
        url: `master/master-inquiry/grouping`,
        method: "POST",
        body: {
          badgeNumber: params.badgeNumber ? Number(params.badgeNumber?.toString().substring(0, 6)) : undefined,
          psnSuffix:
            params.badgeNumber && params.badgeNumber.toString().length > 6
              ? Number(params.badgeNumber?.toString().substring(6))
              : undefined,
          profitYear: params.profitYear,
          endProfitYear: params.endProfitYear,
          startProfitMonth: params.startProfitMonth,
          endProfitMonth: params.endProfitMonth,
          profitCode: params.profitCode,
          contributionAmount: params.contributionAmount,
          earningsAmount: params.earningsAmount,
          forfeitureAmount: params.forfeitureAmount,
          paymentAmount: params.paymentAmount,
          ssn: params.ssn,
          paymentType: params.paymentType,
          memberType: params.memberType,
          name: params.name,
          sortBy: params.pagination.sortBy,
          isSortDescending: params.pagination.isSortDescending,
          skip: params.pagination.skip,
          take: params.pagination.take
        }
      }),
      async onQueryStarted(_params: MasterInquiryRequest, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMasterInquiryGroupingData(data.results));
        } catch (err) {
          console.error("Failed to fetch profit master inquiry grouping:", err);
        }
      }
    })
  })
});

export const {
  useLazyGetProfitMasterInquiryMemberQuery,
  useLazySearchProfitMasterInquiryQuery,
  useLazyGetProfitMasterInquiryMemberDetailsQuery,
  useLazyGetProfitMasterInquiryFilteredDetailsQuery,
  useLazyGetProfitMasterInquiryGroupingQuery
} = InquiryApi;
