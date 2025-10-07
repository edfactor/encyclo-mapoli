import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";
import { MergeProfitsDetailRequest } from "../types/adjustment/adjustment";

const baseQuery = createDataSourceAwareBaseQuery();

export const AdjustmentsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "adjustmentsApi",
  tagTypes: ["MergeOperation"],
  endpoints: (builder) => ({
    mergeProfitsDetail: builder.mutation<void, MergeProfitsDetailRequest>({
      query: (mergeRequest) => ({
        url: "adjustments/merge-profit-details",
        method: "PUT",
        body: mergeRequest
      }),
      invalidatesTags: ["MergeOperation"],
      async onQueryStarted(_mergeRequest, { queryFulfilled, dispatch }) {
        try {
          const { data } = await queryFulfilled;
          console.log("Merge operation completed successfully:", data);
        } catch (err) {
          console.error("Merge operation failed:", err);
          throw err;
        } finally {
        }
      }
    })
  })
});

export const { useMergeProfitsDetailMutation } = AdjustmentsApi;
