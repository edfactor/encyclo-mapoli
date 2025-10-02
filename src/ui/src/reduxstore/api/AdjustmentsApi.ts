import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const AdjustmentsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "adjustmentsApi",
  tagTypes: ["MergeOperation"],
  endpoints: (builder) => ({
    mergeProfitsDetail: builder.mutation<void, MergeProfitsDetailRequest>({
      query: (mergeRequest) => ({
        url: "adjustments/merge-profits-details",
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

export const {
  useMergeProfitsDetailMutation
} = AdjustmentsApi;
