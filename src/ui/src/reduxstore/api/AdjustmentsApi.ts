import { createApi } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery } from "./api";
import { MergeProfitsDetailRequest } from "../types/adjustment/adjustment";
import { setMerging } from "reduxstore/slices/adjustmentsSlice";
import { resetMerge, setMergeSuccess } from "../slices/adjustmentsSlice";


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
          dispatch(resetMerge());  
          dispatch(setMerging(true));
          const { data } = await queryFulfilled;
          console.log("Merge operation completed successfully:", data);
          dispatch(setMergeSuccess());
          dispatch(setMerging(false));
        } catch (err) {
          console.error("Merge operation failed:", err);
          throw err;
        } finally {
            dispatch(setMerging(false));
        }
      }
    })
  })
});

export const {
  useMergeProfitsDetailMutation
} = AdjustmentsApi;
