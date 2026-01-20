import {
  CrossReferenceValidationGroup,
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
      query: (profitYear) => `checksum/master-update/${profitYear}`, // Note: profitYear is a parameter value, not a path segment
      providesTags: (_result, _error, profitYear) => [{ type: "MasterUpdateValidation", id: profitYear }]
    }),
    getProfitSharingReportValidation: builder.query<ValidationResponse, ProfitSharingReportValidationRequest>({
      query: (request) =>
        `checksum/profit-sharing-report/${request.profitYear}/${request.reportSuffix}/${request.useFrozenData ? "true" : "false"}`, // Note: parameter values, not path segments
      providesTags: (_result, _error, request) => [
        { type: "MasterUpdateValidation", id: `${request.profitYear}-${request.reportSuffix}` }
      ]
    }),

    /**
     * Get balance validation data (ALLOC/PAID ALLOC transfers) for a specific profit year.
     * Returns validation results that can be used to display validation icons in the UI.
     *
     * Note: This endpoint uses a different base path (/api/balance-validation/) than other
     * validation endpoints (/api/validation/), hence the relative path navigation.
     *
     * @param profitYear - The profit year to validate (e.g., 2024)
     * @returns CrossReferenceValidationGroup with ALLOC transfer validation results, or null if no data
     *
     * @example
     * ```typescript
     * const { data, isLoading, error } = useGetBalanceValidationQuery(2024);
     * if (data) {
     *   const allocValidation = data.validations.find(v => v.fieldName === 'NetAllocTransfer');
     * }
     * ```
     */
    getBalanceValidation: builder.query<CrossReferenceValidationGroup | null, number>({
      query: (profitYear) => `balance-validation/alloc-transfers/${profitYear}`, // Note: profitYear is a parameter value, not a path segment
      // Handle 404 gracefully - no validation data available for this year
      transformErrorResponse: (response) => {
        if (response.status === 404) {
          return { status: 404, data: null };
        }
        return response;
      }
    })
  })
});

export const {
  useGetMasterUpdateValidationQuery,
  useLazyGetMasterUpdateValidationQuery,
  useGetProfitSharingReportValidationQuery,
  useLazyGetProfitSharingReportValidationQuery,
  useGetBalanceValidationQuery,
  useLazyGetBalanceValidationQuery
} = validationApi;
