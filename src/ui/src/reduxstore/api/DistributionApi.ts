import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";
import {
  DistributionSearchRequest,
  DistributionSearchResultDto,
  CreateDistributionRequest,
  EditDistributionRequest,
  CreateOrUpdateDistributionResponse
} from "../../types";

const baseQuery = createDataSourceAwareBaseQuery();

export const DistributionApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "distributionApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    searchDistributions: builder.query<DistributionSearchResultDto, DistributionSearchRequest>({
      query: (request) => ({
        url: "/distribution/search",
        method: "POST",
        body: request
      })
    }),
    createDistribution: builder.mutation<CreateOrUpdateDistributionResponse, CreateDistributionRequest>({
      query: (request) => ({
        url: "/distribution",
        method: "POST",
        body: request
      })
    }),
    updateDistribution: builder.mutation<CreateOrUpdateDistributionResponse, EditDistributionRequest>({
      query: (request) => ({
        url: "/distribution",
        method: "PUT",
        body: request
      })
    }),
    deleteDistribution: builder.mutation<boolean, number>({
      query: (id) => ({
        url: `/distribution/${id}`,
        method: "DELETE"
      })
    })
  })
});

export const {
  useLazySearchDistributionsQuery,
  useCreateDistributionMutation,
  useUpdateDistributionMutation,
  useDeleteDistributionMutation
} = DistributionApi;
