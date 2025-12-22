import { createApi } from "@reduxjs/toolkit/query/react";

import { setBeneficiaryError } from "reduxstore/slices/beneficiarySlice";
import {
  BeneficiariesGetAPIResponse,
  BeneficiaryDetail,
  BeneficiaryDetailAPIRequest,
  BeneficiarySearchAPIRequest,
  BeneficiaryTypesRequestDto,
  BeneficiaryTypesResponseDto,
  CreateBeneficiaryContactRequest,
  CreateBeneficiaryContactResponse,
  CreateBeneficiaryRequest,
  CreateBeneficiaryResponse,
  DeleteBeneficiaryRequest,
  UpdateBeneficiaryRequest,
  UpdateBeneficiaryResponse
} from "reduxstore/types";
import { Paged } from "smart-ui-library";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const BeneficiariesApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "beneficiariesApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getBeneficiaries: builder.query<BeneficiariesGetAPIResponse, BeneficiaryDetailAPIRequest>({
      query: (request) => ({
        url: `/beneficiaries`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
          //dispatch(setBeneficiary(data));
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),
    beneficiarySearchFilter: builder.query<Paged<BeneficiaryDetail>, BeneficiarySearchAPIRequest>({
      query: (request) => ({
        url: `/beneficiaries/search`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),
    getBeneficiarytypes: builder.query<BeneficiaryTypesResponseDto, BeneficiaryTypesRequestDto>({
      query: (request) => ({
        url: `/beneficiaryType`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),
    createBeneficiaries: builder.mutation<Paged<CreateBeneficiaryResponse>, CreateBeneficiaryRequest>({
      query: (request) => ({
        url: `/beneficiaries`,
        method: "POST",
        body: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to create beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to create beneficiaries"));
        }
      }
    }),
    createBeneficiaryContact: builder.mutation<CreateBeneficiaryContactResponse, CreateBeneficiaryContactRequest>({
      query: (request) => ({
        url: `/beneficiaries/contact`,
        method: "POST",
        body: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to create beneficiary contact", err);
          dispatch(setBeneficiaryError("Failed to create beneficiary contact"));
        }
      }
    }),
    updateBeneficiary: builder.mutation<UpdateBeneficiaryResponse, UpdateBeneficiaryRequest>({
      query: (request) => ({
        url: `/beneficiaries`,
        method: "PUT",
        body: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to update beneficiary", err);
          dispatch(setBeneficiaryError("Failed to update beneficiary"));
        }
      }
    }),
    getBeneficiaryDetail: builder.query<BeneficiaryDetail, BeneficiaryDetailAPIRequest>({
      query: (request) => ({
        url: `/beneficiaries/detail`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),

    deleteBeneficiary: builder.mutation<{ success: boolean; message?: string }, DeleteBeneficiaryRequest>({
      query: (request) => ({
        url: `/beneficiaries/${request.id}`,
        method: "DELETE"
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to delete beneficiary", err);
          dispatch(setBeneficiaryError("Failed to delete beneficiary"));
        }
      }
    })
  })
});

export const {
  // Queries (GET operations)
  useLazyGetBeneficiaryDetailQuery,
  useLazyBeneficiarySearchFilterQuery,
  useLazyGetBeneficiariesQuery,
  useLazyGetBeneficiarytypesQuery,
  // Mutations (POST/PUT/DELETE operations)
  useCreateBeneficiariesMutation,
  useCreateBeneficiaryContactMutation,
  useUpdateBeneficiaryMutation,
  useDeleteBeneficiaryMutation
} = BeneficiariesApi;
