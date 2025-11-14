import { createApi } from "@reduxjs/toolkit/query/react";
import { MergeProfitsDetailRequest } from "@/types/adjustment/adjustment";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const AdjustmentsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "adjustmentsApi",
  tagTypes: ["MergeOperation"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    mergeProfitsDetail: builder.mutation<void, MergeProfitsDetailRequest>({
      query: (mergeRequest) => ({
        url: "adjustments/merge-profit-details",
        method: "PUT",
        body: mergeRequest
      }),
      invalidatesTags: ["MergeOperation"],
      async onQueryStarted(_mergeRequest, { queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          console.log("Merge operation completed successfully:", data);
        } catch (err) {
          console.error("Merge operation failed:", err);
          throw err;
        }
      }
    })
  })
});

export const { useMergeProfitsDetailMutation } = AdjustmentsApi;
