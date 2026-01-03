import { createApi } from "@reduxjs/toolkit/query/react";

import {
  CommentTypeDto,
  CreateCommentTypeRequest,
  RmdFactorDto,
  UpdateCommentTypeRequest,
  UpdateRmdFactorRequest
} from "../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const AdministrationApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "administrationApi",
  tagTypes: ["CommentTypes", "RmdFactors"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getCommentTypes: builder.query<CommentTypeDto[], void>({
      query: () => ({
        url: "administration/comment-types",
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
    createCommentType: builder.mutation<CommentTypeDto, CreateCommentTypeRequest>({
      query: (body) => ({
        url: "administration/comment-types",
        method: "POST",
        body
      }),
      invalidatesTags: ["CommentTypes"]
    }),
    updateCommentType: builder.mutation<CommentTypeDto, UpdateCommentTypeRequest>({
      query: (body) => ({
        url: "administration/comment-types",
        method: "PUT",
        body
      }),
      invalidatesTags: ["CommentTypes"]
    }),

    // RMD Factors endpoints
    getRmdFactors: builder.query<RmdFactorDto[], void>({
      query: () => ({
        url: "/administration/rmd-factors",
        method: "GET"
      }),
      transformResponse: (response: RmdFactorDto[] | { items: RmdFactorDto[]; count: number }) => {
        // Handle both direct array and paginated response formats
        if (Array.isArray(response)) {
          return response;
        }
        return response.items || [];
      },
      providesTags: ["RmdFactors"]
    }),
    updateRmdFactor: builder.mutation<RmdFactorDto, UpdateRmdFactorRequest>({
      query: (body) => ({
        url: "/administration/rmd-factors",
        method: "PUT",
        body
      }),
      invalidatesTags: ["RmdFactors"]
    })
  })
});

export const {
  useGetCommentTypesQuery,
  useCreateCommentTypeMutation,
  useUpdateCommentTypeMutation,
  useGetRmdFactorsQuery,
  useUpdateRmdFactorMutation
} = AdministrationApi;
