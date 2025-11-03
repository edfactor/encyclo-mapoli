import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { MasterUpdateCrossReferenceValidationResponse } from "@/types/validation/cross-reference-validation";
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
      providesTags: (result, error, profitYear) => [{ type: "MasterUpdateValidation", id: profitYear }]
    })
  })
});

export const { useGetMasterUpdateValidationQuery, useLazyGetMasterUpdateValidationQuery } = validationApi;
