import { createApi } from "@reduxjs/toolkit/query/react";

import { CreateMilitaryContributionRequest, MasterInquiryDetail, MilitaryContributionRequest } from "reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";
import { setMilitaryContributions, setMilitaryError } from "reduxstore/slices/militarySlice";
import { Paged } from "smart-ui-library";

const baseQuery = createDataSourceAwareBaseQuery();
export const MilitaryApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "militaryApi",
  endpoints: (builder) => ({
    getMilitaryContributions: builder.query<
      Paged<MasterInquiryDetail>,
      MilitaryContributionRequest & { archive?: boolean }
    >({
      query: (request) => ({
        url: `military${request.archive ? "?archive=true" : ""}`,
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
