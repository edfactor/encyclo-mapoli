import { createApi } from "@reduxjs/toolkit/query/react";

import { Paged } from "smart-ui-library";
import { setFrozenStateCollectionResponse, setFrozenStateResponse } from "../../reduxstore/slices/frozenSlice";
import {
  AnnuityRateDto,
  AuditChangeEntryDto,
  AuditEventDto,
  AuditSearchRequestDto,
  CurrentUserResponseDto,
  FakeTimeStatusResponse,
  FreezeDemographicsRequest,
  FrozenStateResponse,
  RowCountResult,
  SetFakeTimeRequest,
  SortedPaginationRequestDto,
  StateTaxRateDto,
  UpdateAnnuityRateRequest,
  UpdateStateTaxRateRequest
} from "../../reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

type GetAnnuityRatesQueryArgs = {
  sortBy: string;
  isSortDescending: boolean;
};

export const ItOperationsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "itOperationsApi",
  tagTypes: ["FrozenState"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getAnnuityRates: builder.query<AnnuityRateDto[], GetAnnuityRatesQueryArgs>({
      query: (params) => ({
        url: "administration/annuity-rates",
        method: "GET",
        params: {
          sortBy: params.sortBy,
          isSortDescending: params.isSortDescending
        }
      })
    }),
    updateAnnuityRate: builder.mutation<AnnuityRateDto, UpdateAnnuityRateRequest>({
      query: (request) => ({
        url: "administration/annuity-rates",
        method: "PUT",
        body: request
      })
    }),
    getStateTaxRates: builder.query<StateTaxRateDto[], void>({
      query: () => ({
        url: "administration/state-tax-rates",
        method: "GET"
      })
    }),
    updateStateTaxRate: builder.mutation<StateTaxRateDto, UpdateStateTaxRateRequest>({
      query: (request) => ({
        url: "administration/state-tax-rates",
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
    }),
    getFakeTimeStatus: builder.query<FakeTimeStatusResponse, void>({
      query: () => ({
        url: "itdevops/fake-time/status",
        method: "GET"
      })
    }),
    validateFakeTime: builder.mutation<FakeTimeStatusResponse, SetFakeTimeRequest>({
      query: (request) => ({
        url: "itdevops/fake-time/validate",
        method: "POST",
        body: request
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
  useFreezeDemographicsMutation,
  useLazyGetMetadataQuery,
  useLazyGetCurrentUserQuery,
  useLazySearchAuditQuery,
  useLazyGetAuditChangesQuery,
  useGetFakeTimeStatusQuery,
  useLazyGetFakeTimeStatusQuery,
  useValidateFakeTimeMutation
} = ItOperationsApi;
