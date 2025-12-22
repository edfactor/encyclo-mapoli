import { createApi } from "@reduxjs/toolkit/query/react";

import {
    CommentTypeDto,
    UpdateCommentTypeRequest
} from "../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const AdministrationApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "administrationApi",
  tagTypes: ["CommentTypes"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getCommentTypes: builder.query<CommentTypeDto[], void>({
      query: () => ({
        url: "/administration/comment-types",
        method: "GET"
      }),
      transformResponse: (response: CommentTypeDto[] | { items: CommentTypeDto[]; count: number }) => {
        // Handle both direct array and paginated response formats
        if (Array.isArray(response)) {
          return response;
        }
        return response.items || [];
      },
      providesTags: ["CommentTypes"]
    }),
    updateCommentType: builder.mutation<CommentTypeDto, UpdateCommentTypeRequest>({
      query: (body) => ({
        url: "/administration/comment-types",
        method: "PUT",
        body
      }),
      invalidatesTags: ["CommentTypes"]
    })
  })
});

export const {
  useGetCommentTypesQuery,
  useUpdateCommentTypeMutation
} = AdministrationApi;
