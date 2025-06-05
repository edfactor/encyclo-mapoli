import { createApi } from "@reduxjs/toolkit/query/react";
import {
  MasterInquiryRequest,
  MasterInquiryMemberRequest,
  MasterInquiryResponseDto,
  PagedReportResponse,
  EmployeeDetails
} from "../types";
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
    // url is master-inquiry/grouping - no member type or id
    getProfitMasterInquiryGrouping: builder.query<PagedReportResponse<MasterInquiryResponseDto>, MasterInquiryRequest>({
      query: (params) => ({
        url: `master/master-inquiry/grouping`,
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
      }),
      async onQueryStarted(params: MasterInquiryRequest, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMasterInquiryGroupingData(data.response.results));
        } catch (err) {
          console.log("Err: " + err);
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
