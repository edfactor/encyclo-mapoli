import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
  FrozenStateResponse,
  SortedPaginationRequestDto,
  FreezeDemographicsRequest,
  RowCountResult,
  CurrentUserResponseDto
} from "reduxstore/types";
import { setFrozenStateResponse, setFrozenStateCollectionResponse } from "reduxstore/slices/frozenSlice";
import { url } from "./api";
import { Paged } from "smart-ui-library";

export const ItOperations = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/`,
    prepareHeaders: (headers, { getState }) => {
      const root = getState() as RootState;
      const token = root.security.token;
      const impersonating = root.security.impersonating;
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
    }),
  })
});

export const {
  useLazyGetFrozenStateResponseQuery,
  useLazyGetHistoricalFrozenStateResponseQuery,
  useFreezeDemographicsMutation,
  useLazyGetMetadataQuery,
  useLazyGetCurrentUserQuery
} = ItOperations;
