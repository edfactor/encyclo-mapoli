import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
  CreateMilitaryContributionRequest,
  MasterInquiryDetail,
  MilitaryContributionRequest,
  PagedReportResponse
} from "reduxstore/types";
import { url } from "./api";
import { setMilitaryContributions, setMilitaryError } from "reduxstore/slices/militarySlice";

export const MilitaryApi = createApi({
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
  reducerPath: "militaryApi",
  endpoints: (builder) => ({
    getMilitaryContributions: builder.query<PagedReportResponse<MasterInquiryDetail>, MilitaryContributionRequest>({
      query: (request) => ({
        url: `military`,
        method: "GET",
        params: {
          badgeNumber: request.badgeNumber,
          profitYear: request.profitYear,
          skip: request.pagination.skip,
          take: request.pagination.take
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMilitaryContributions(data));
        } catch (err) {
          console.error("Failed to fetch military contributions:", err);
          dispatch(setMilitaryError("Failed to fetch military contributions"));
        }
      }
    }),
    createMilitaryContribution: builder.mutation<MasterInquiryDetail, CreateMilitaryContributionRequest>({
      query: (request) => ({
        url: "military",
        method: "POST",
        body: request
      })
    })
  })
});

export const { useLazyGetMilitaryContributionsQuery, useCreateMilitaryContributionMutation } = MilitaryApi;
