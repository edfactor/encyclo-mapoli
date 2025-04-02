import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { setMasterInquiryData } from "reduxstore/slices/inquirySlice";
import { RootState } from "reduxstore/store";
import { MasterInquiryRequest, MasterInquiryResponseType } from "reduxstore/types";
import { url } from "./api";

export const InquiryApi = createApi({
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
        if (localImpersonation) {
          headers.set("impersonation", localImpersonation);
        }
      }
      return headers;
    }
  }),
  reducerPath: "inquiryApi",
  endpoints: (builder) => ({
    getProfitMasterInquiry: builder.query<MasterInquiryResponseType, MasterInquiryRequest>({
      query: (params) => ({
        url: "master/master-inquiry",
        method: "POST",
        body: {
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
    })
  })
});

export const { useLazyGetProfitMasterInquiryQuery } = InquiryApi;
