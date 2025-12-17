import { createApi } from "@reduxjs/toolkit/query/react";

import {
  GetProfitSharingAdjustmentsRequest,
  GetProfitSharingAdjustmentsResponse,
  SaveProfitSharingAdjustmentsRequest
} from "../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const ProfitDetailsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "profitDetailsApi",
  tagTypes: ["ProfitSharingAdjustments"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getProfitSharingAdjustments: builder.query<GetProfitSharingAdjustmentsResponse, GetProfitSharingAdjustmentsRequest>({
      query: (params) => ({
        url: "adjustments/under21",
        method: "GET",
        params
      }),
      providesTags: (_result, _error, args) => [
        {
          type: "ProfitSharingAdjustments" as const,
          id: `${args.profitYear}-${args.badgeNumber}`
        }
      ]
    }),
    saveProfitSharingAdjustments: builder.mutation<
      GetProfitSharingAdjustmentsResponse,
      SaveProfitSharingAdjustmentsRequest
    >({
      query: (body) => ({
        url: "adjustments/under21",
        method: "POST",
        body
      }),
      invalidatesTags: (_result, _error, args) => [
        {
          type: "ProfitSharingAdjustments" as const,
          id: `${args.profitYear}-${args.badgeNumber}`
        }
      ]
    })
  })
});

export const { useLazyGetProfitSharingAdjustmentsQuery, useSaveProfitSharingAdjustmentsMutation } = ProfitDetailsApi;
