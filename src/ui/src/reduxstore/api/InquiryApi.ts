import { createApi } from "@reduxjs/toolkit/query/react";
import {
  MasterInquiryRequest,
  MasterInquiryMemberRequest,
  MasterInquiryResponseDto,
  PagedReportResponse,
  EmployeeDetails,
  GroupedProfitSummaryDto
} from "../types";
import { Paged } from "smart-ui-library";
import { createDataSourceAwareBaseQuery } from "./api";
import { setMasterInquiryGroupingData } from "reduxstore/slices/inquirySlice";

const baseQuery = createDataSourceAwareBaseQuery();
export const InquiryApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "inquiryApi",
  endpoints: (builder) => ({
    // Master Inquiry API endpoints
    searchProfitMasterInquiry: builder.query<PagedReportResponse<EmployeeDetails>, MasterInquiryRequest>({
      query: (params) => ({
        url: "master/master-inquiry/search",
        method: "POST",
        body: {
          badgeNumber: Number(params.badgeNumber?.toString().substring(0, 6)),
          psnSuffix: Number(params.badgeNumber?.toString().substring(6)),
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
          isSortDescending: params.pagination.isSortDescending
        }
      })
    }),
    getProfitMasterInquiryMember: builder.query<EmployeeDetails, MasterInquiryMemberRequest>({
      query: (params) => ({
        url: "master/master-inquiry/member",
        method: "POST",
        body: params
      })
    }),
    getProfitMasterInquiryMemberDetails: builder.query<PagedReportResponse<MasterInquiryResponseDto>, { memberType: number; id: number; skip?: number; take?: number; sortBy?: string; isSortDescending?: boolean }>({
      query: ({ memberType, id, ...pagination }) => ({
        url: `master/master-inquiry/member/${memberType}/${id}/details`,
        method: "GET",
        params: pagination
      })
    }),
    getProfitMasterInquiryGrouping: builder.query<Paged<GroupedProfitSummaryDto>, MasterInquiryRequest>({
      query: (params) => ({
        url: `master/master-inquiry/grouping`,
        method: "POST",
        body: {
          badgeNumber: params.badgeNumber ? Number(params.badgeNumber?.toString().substring(0, 6)) : undefined,
          psnSuffix: params.badgeNumber && params.badgeNumber.toString().length > 6 ? Number(params.badgeNumber?.toString().substring(6)) : undefined,
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
      async onQueryStarted(params: MasterInquiryRequest, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMasterInquiryGroupingData(data.results));
        } catch (err) {
          console.log("Error in getProfitMasterInquiryGrouping:", err);
        }
      }
    })
  })
});

export const {
  useLazyGetProfitMasterInquiryMemberQuery,
  useLazySearchProfitMasterInquiryQuery,
  useLazyGetProfitMasterInquiryMemberDetailsQuery,
  useLazyGetProfitMasterInquiryGroupingQuery
} = InquiryApi;
