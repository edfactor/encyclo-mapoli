import { createApi } from "@reduxjs/toolkit/query/react";

import { setMilitaryContributions, setMilitaryError } from "reduxstore/slices/militarySlice";
import { CreateMilitaryContributionRequest, MasterInquiryDetail, MilitaryContributionRequest } from "reduxstore/types";
import { Paged } from "smart-ui-library";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const MilitaryApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "militaryApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
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
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMilitaryContributions(data));
        } catch (err) {
          console.error("Failed to fetch military contributions:", err);
          dispatch(setMilitaryError("Failed to fetch military contributions"));
        }
      }
    }),
    createMilitaryContribution: builder.mutation<
      MasterInquiryDetail,
      CreateMilitaryContributionRequest & { suppressAllToastErrors?: boolean; onlyNetworkToastErrors?: boolean }
    >({
      query: (request) => {
        const { suppressAllToastErrors, onlyNetworkToastErrors } = request;
        return {
          url: "military",
          method: "POST",
          body: request,
          meta: { suppressAllToastErrors, onlyNetworkToastErrors }
        };
      }
    })
  })
});

export const { useLazyGetMilitaryContributionsQuery, useCreateMilitaryContributionMutation } = MilitaryApi;
