import { createApi } from "@reduxjs/toolkit/query/react";

import { Paged } from "smart-ui-library";
import {
  setFrozenStateCollectionResponse,
  setFrozenStateResponse,
  setProfitYearSelectorData
} from "../../reduxstore/slices/frozenSlice";
import {
  AnnuityRateDto,
  AuditChangeEntryDto,
  AuditEventDto,
  AuditSearchRequestDto,
  CurrentUserResponseDto,
  FreezeDemographicsRequest,
  FrozenStateResponse,
  RowCountResult,
  SortedPaginationRequestDto,
  StateTaxRateDto,
  UpdateAnnuityRateRequest,
  UpdateStateTaxRateRequest
} from "../../reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const ItOperationsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "itOperationsApi",
  tagTypes: ["FrozenState"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getAnnuityRates: builder.query<AnnuityRateDto[], void>({
      query: () => ({
        url: "itdevops/annuity-rates",
        method: "GET"
      })
    }),
    updateAnnuityRate: builder.mutation<AnnuityRateDto, UpdateAnnuityRateRequest>({
      query: (request) => ({
        url: "itdevops/annuity-rates",
        method: "PUT",
        body: request
      })
    }),
    getStateTaxRates: builder.query<StateTaxRateDto[], void>({
      query: () => ({
        url: "itdevops/state-tax-rates",
        method: "GET"
      })
    }),
    updateStateTaxRate: builder.mutation<StateTaxRateDto, UpdateStateTaxRateRequest>({
      query: (request) => ({
        url: "itdevops/state-tax-rates",
        method: "PUT",
        body: request
      })
    }),
    getFrozenStateResponse: builder.query<FrozenStateResponse, void>({
      query: () => ({
        url: `itdevops/frozen/active`,
        method: "GET"
      }),
      providesTags: ["FrozenState"],
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
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
        url: `itdevops/frozen`,
        method: "GET",
        params: {
          take: params.take,
          skip: params.skip,
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      }),
      providesTags: ["FrozenState"],
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setFrozenStateCollectionResponse(data));
        } catch (err) {
          console.error("Failed to fetch frozen state collection:", err);
          dispatch(setFrozenStateCollectionResponse(null)); // Handle API errors
        }
      }
    }),
    getProfitYearSelectorFrozenData: builder.query<Paged<FrozenStateResponse>, SortedPaginationRequestDto>({
      query: (params) => ({
        url: `itdevops/frozen`,
        method: "GET",
        params: {
          take: params.take,
          skip: params.skip,
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setProfitYearSelectorData(data));
        } catch (err) {
          console.error("Failed to fetch profit year selector data:", err);
          dispatch(setProfitYearSelectorData(null)); // Handle API errors
        }
      }
    }),
    freezeDemographics: builder.mutation<void, FreezeDemographicsRequest>({
      query: (request) => ({
        url: "itdevops/freeze",
        method: "POST",
        body: request
      }),
      invalidatesTags: ["FrozenState"]
    }),
    getMetadata: builder.query<RowCountResult[], void>({
      query: () => ({
        url: `itdevops/metadata`,
        method: "GET"
      })
    }),
    getCurrentUser: builder.query<CurrentUserResponseDto, void>({
      query: () => ({
        url: `common/current-user`,
        method: "GET"
      })
    }),
    searchAudit: builder.query<Paged<AuditEventDto>, AuditSearchRequestDto>({
      query: (params) => ({
        url: "audit/search",
        method: "GET",
        params: {
          tableName: params.tableName,
          operation: params.operation,
          userName: params.userName,
          startTime: params.startTime,
          endTime: params.endTime,
          take: params.take,
          skip: params.skip,
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      })
    }),
    getAuditChanges: builder.query<AuditChangeEntryDto[], number>({
      query: (auditEventId) => ({
        url: `audit/changes/${auditEventId}`,
        method: "GET"
      })
    })
  })
});

export const {
  useGetAnnuityRatesQuery,
  useUpdateAnnuityRateMutation,
  useGetStateTaxRatesQuery,
  useUpdateStateTaxRateMutation,
  useLazyGetFrozenStateResponseQuery,
  useLazyGetHistoricalFrozenStateResponseQuery,
  useLazyGetProfitYearSelectorFrozenDataQuery,
  useFreezeDemographicsMutation,
  useLazyGetMetadataQuery,
  useLazyGetCurrentUserQuery,
  useLazySearchAuditQuery,
  useLazyGetAuditChangesQuery
} = ItOperationsApi;
