import { createApi } from "@reduxjs/toolkit/query/react";
import {
  CreateDistributionRequest,
  CreateOrUpdateDistributionResponse,
  DistributionSearchRequest,
  DistributionSearchResultDto,
  EditDistributionRequest
} from "../../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const DistributionApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "distributionApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    searchDistributions: builder.query<
      DistributionSearchResultDto,
      DistributionSearchRequest & { suppressAllToastErrors?: boolean; onlyNetworkToastErrors?: boolean }
    >({
      query: (params) => {
        const { suppressAllToastErrors, onlyNetworkToastErrors, ...requestData } = params;
        return {
          url: "/distributions/search",
          method: "POST",
          body: requestData,
          // Pass params through meta so middleware can access it
          meta: { suppressAllToastErrors, onlyNetworkToastErrors }
        };
      }
    }),
    createDistribution: builder.mutation<CreateOrUpdateDistributionResponse, CreateDistributionRequest>({
      query: (request) => ({
        url: "/distributions",
        method: "POST",
        body: request
      })
    }),
    updateDistribution: builder.mutation<CreateOrUpdateDistributionResponse, EditDistributionRequest>({
      query: (request) => ({
        url: "/distributions",
        method: "PUT",
        body: request
      })
    }),
    deleteDistribution: builder.mutation<boolean, number>({
      query: (id) => ({
        url: `/distributions/${id}`,
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
