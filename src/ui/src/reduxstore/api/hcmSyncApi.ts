import { createApi } from "@reduxjs/toolkit/query/react";
import { ClearAuditResponse, DemographicSyncAuditPage, OracleHcmSyncMetadata } from "../../pages/ITOperations/OracleHcmDiagnostics/types";
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
        url: "itdevops/oracleHcm/metadata",
        method: "GET"
      }),
      providesTags: ["hcm-sync-metadata"]
    }),

    /**
     * Fetches paginated demographic sync audit records
     */
    getDemographicSyncAudit: builder.query<
      DemographicSyncAuditPage,
      { pageNumber?: number; pageSize?: number }
    >({
      query: ({ pageNumber = 1, pageSize = 50 }) => ({
        url: "itdevops/oracleHcm/audit",
        method: "GET",
        params: {
          pageNumber,
          pageSize
        }
      }),
      providesTags: ["demographic-sync-audit"]
    }),

    /**
     * Clears all demographic sync audit records
     */
    clearDemographicSyncAudit: builder.mutation<ClearAuditResponse, void>({
      query: () => ({
        url: "itdevops/oracleHcm/audit/clear",
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
