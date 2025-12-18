import { createApi } from "@reduxjs/toolkit/query/react";
import { Paged } from "smart-ui-library";
import type {
    ClearAuditResponse,
    DemographicSyncAuditRecord,
    OracleHcmSyncMetadata,
    SortedPaginationRequestDto
} from "../../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const hcmSyncApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "hcmSyncApi",
  tagTypes: ["hcm-sync-metadata", "demographic-sync-audit"],
  endpoints: (builder) => ({
    /**
     * Fetches OracleHcm sync metadata with four timestamp fields
     */
    getOracleHcmSyncMetadata: builder.query<OracleHcmSyncMetadata, void>({
      query: () => ({
        url: "administration/oracleHcm/metadata",
        method: "GET"
      }),
      providesTags: ["hcm-sync-metadata"]
    }),

    /**
     * Fetches paginated demographic sync audit records
     */
    getDemographicSyncAudit: builder.query<Paged<DemographicSyncAuditRecord>, SortedPaginationRequestDto>({
      query: ({ skip, take, sortBy, isSortDescending }) => {
        return {
          url: "administration/oracleHcm/audit",
          method: "GET",
          params: {
            skip,
            take,
            sortBy,
            isSortDescending
          }
        };
      },
      providesTags: ["demographic-sync-audit"]
    }),

    /**
     * Clears all demographic sync audit records
     */
    clearDemographicSyncAudit: builder.mutation<ClearAuditResponse, void>({
      query: () => ({
        url: "administration/oracleHcm/audit/clear",
        method: "POST"
      }),
      invalidatesTags: ["hcm-sync-metadata", "demographic-sync-audit"]
    })
  })
});

export const {
  useGetOracleHcmSyncMetadataQuery,
  useLazyGetOracleHcmSyncMetadataQuery,
  useGetDemographicSyncAuditQuery,
  useLazyGetDemographicSyncAuditQuery,
  useClearDemographicSyncAuditMutation
} = hcmSyncApi;

// Shorter aliases for convenience
export const useGetOracleHcmSyncMetadata = useGetOracleHcmSyncMetadataQuery;
export const useLazyGetDemographicSyncAudit = useLazyGetDemographicSyncAuditQuery;
export const useClearDemographicSyncAudit = useClearDemographicSyncAuditMutation;
