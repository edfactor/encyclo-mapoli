import { createApi } from "@reduxjs/toolkit/query/react";

import { setMasterInquiryData } from "reduxstore/slices/inquirySlice";
import {
  MasterInquiryRequest,
  MasterInquiryMemberRequest,
  MasterInquiryResponseType,
  MasterInquiryResponseDto,
  MemberDetails,
  PagedReportResponse
} from "../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const InquiryApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "inquiryApi",
  endpoints: (builder) => ({
    getProfitMasterInquiry: builder.query<MasterInquiryResponseType, MasterInquiryRequest>({
      query: (params) => ({
        url: "master/master-inquiry",
        method: "POST",
        body: {
          badgeNumber: Number(params.badgeNumber?.toString().substring(0, 6)),
          psnSuffix: Number(params.badgeNumber?.toString().substring(6)),
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMasterInquiryData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    // Master Inquiry API endpoints
    searchProfitMasterInquiry: builder.query<PagedReportResponse<MemberDetails>, MasterInquiryRequest>({
      query: (params) => ({
        url: "master/master-inquiry/search",
        method: "POST",
        body: params
      })
    }),
    getProfitMasterInquiryMember: builder.query<MemberDetails, MasterInquiryMemberRequest>({
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
    })
  })
});

export const { useLazyGetProfitMasterInquiryQuery } = InquiryApi;
