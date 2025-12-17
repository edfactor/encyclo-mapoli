import {
  MasterUpdateCrossReferenceValidationResponse,
  ProfitSharingReportValidationRequest,
  ValidationResponse
} from "@/types/validation/cross-reference-validation";
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { prepareHeaders, url } from "./api";

/**
 * RTK Query API for validation endpoints.
 * Provides reusable validation data fetching for cross-reference checksums.
 */
export const validationApi = createApi({
  reducerPath: "validationApi",
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/validation/`,
    prepareHeaders
  }),
  tagTypes: ["MasterUpdateValidation"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    /**
     * Get Master Update cross-reference validation data for a specific profit year.
     * Returns validation results for all checksum groups: Contributions, Earnings, Forfeitures, Distributions, and ALLOC transfers.
     *
     * @param profitYear - The profit year to validate (e.g., 2024)
     * @returns Comprehensive validation data with per-field validation results
     *
     * @example
     * ```typescript
     * const { data, isLoading, error } = useGetMasterUpdateValidationQuery(2024);
     * ```
     */
    getMasterUpdateValidation: builder.query<MasterUpdateCrossReferenceValidationResponse, number>({
      query: (profitYear) => `checksum/master-update/${profitYear}`,
      providesTags: (_result, _error, profitYear) => [{ type: "MasterUpdateValidation", id: profitYear }]
    }),
    getProfitSharingReportValidation: builder.query<ValidationResponse, ProfitSharingReportValidationRequest>({
      query: (request) => `checksum/profit-sharing-report/${request.profitYear}/${request.reportSuffix}`,
      providesTags: (_result, _error, request) => [
        { type: "MasterUpdateValidation", id: `${request.profitYear}-${request.reportSuffix}` }
      ]
    })
  })
});

export const {
  useGetMasterUpdateValidationQuery,
  useLazyGetMasterUpdateValidationQuery,
  useGetProfitSharingReportValidationQuery,
  useLazyGetProfitSharingReportValidationQuery
} = validationApi;
