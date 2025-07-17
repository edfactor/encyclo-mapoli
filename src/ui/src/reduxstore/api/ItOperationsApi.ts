import { createApi } from "@reduxjs/toolkit/query/react";

import {
  FrozenStateResponse,
  SortedPaginationRequestDto,
  FreezeDemographicsRequest,
  RowCountResult,
  CurrentUserResponseDto
} from "reduxstore/types";
import { setFrozenStateResponse, setFrozenStateCollectionResponse } from "reduxstore/slices/frozenSlice";
import { createDataSourceAwareBaseQuery } from "./api";
import { Paged } from "smart-ui-library";

const baseQuery = createDataSourceAwareBaseQuery();
export const ItOperationsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "itOperationsApi",
  endpoints: (builder) => ({
    getFrozenStateResponse: builder.query<FrozenStateResponse, void>({
      query: () => ({
        url: `itoperations/frozen/active`,
        method: "GET"
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setFrozenStateResponse(data));
        } catch (err) {
          console.error("Failed to fetch frozen state:", err);
          dispatch(setFrozenStateResponse(null)); // Handle API errors
        }
      }
    }),
    getHistoricalFrozenStateResponse: builder.query<Paged<FrozenStateResponse>, SortedPaginationRequestDto>({
      query: (params) => ({
        url: `itoperations/frozen`,
        method: "GET",
        params: {
          take: params.take,
          skip: params.skip,
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setFrozenStateCollectionResponse(data));
        } catch (err) {
          console.error("Failed to fetch frozen state collection:", err);
          dispatch(setFrozenStateCollectionResponse(null)); // Handle API errors
        }
      }
    }),
    freezeDemographics: builder.mutation<void, FreezeDemographicsRequest>({
      query: (request) => ({
        url: "itoperations/freeze",
        method: "POST",
        body: request
      })
    }),
    getMetadata: builder.query<RowCountResult[], void>({
      query: () => ({
        url: `itoperations/metadata`,
        method: "GET"
      })
    }),
    getCurrentUser: builder.query<CurrentUserResponseDto, void>({
      query: () => ({
        url: `common/current-user`,
        method: "GET"
      })
    })
  })
});

export const {
  useLazyGetFrozenStateResponseQuery,
  useLazyGetHistoricalFrozenStateResponseQuery,
  useFreezeDemographicsMutation,
  useLazyGetMetadataQuery,
  useLazyGetCurrentUserQuery
} = ItOperationsApi;
