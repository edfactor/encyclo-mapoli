import { createApi } from "@reduxjs/toolkit/query/react";

import { setBeneficiaryError } from "reduxstore/slices/beneficiarySlice";
import {
  BeneficiaryKindRequestDto,
  BeneficiaryKindResponseDto,
  BeneficiaryRequestDto,
  BeneficiaryResponse,
  BeneficiarySearchFilterRequest,
  BeneficiarySearchFilterResponse,
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
  endpoints: (builder) => ({
    getBeneficiaries: builder.query<BeneficiaryResponse, BeneficiaryRequestDto>({
      query: (request) => ({
        url: `/beneficiaries`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
          //dispatch(setBeneficiary(data));
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),
    beneficiarySearchFilter: builder.query<Paged<BeneficiarySearchFilterResponse>, BeneficiarySearchFilterRequest>({
      query: (request) => ({
        url: `/beneficiaries/search`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),
    getBeneficiaryKind: builder.query<BeneficiaryKindResponseDto, BeneficiaryKindRequestDto>({
      query: (request) => ({
        url: `/beneficiaryKind`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),
    createBeneficiaries: builder.query<Paged<CreateBeneficiaryResponse>, CreateBeneficiaryRequest>({
      query: (request) => ({
        url: `/beneficiaries`,
        method: "POST",
        body: request
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to create beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to create beneficiaries"));
        }
      }
    }),
    createBeneficiaryContact: builder.query<CreateBeneficiaryContactResponse, CreateBeneficiaryContactRequest>({
      query: (request) => ({
        url: `/beneficiaries/contact`,
        method: "POST",
        body: request
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to create beneficiary contact", err);
          dispatch(setBeneficiaryError("Failed to create beneficiary contact"));
        }
      }
    }),
    updateBeneficiary: builder.query<UpdateBeneficiaryResponse, UpdateBeneficiaryRequest>({
      query: (request) => ({
        url: `/beneficiaries`,
        method: "PUT",
        body: request
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to update beneficiary", err);
          dispatch(setBeneficiaryError("Failed to update beneficiary"));
        }
      }
    }),
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    getBeneficiaryDetail: builder.query<any, any>({
      query: (request) => ({
        url: `/beneficiaries/detail`,
        method: "GET",
        params: request
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
        } catch (err) {
          console.error("Failed to fetch beneficiaries:", err);
          dispatch(setBeneficiaryError("Failed to fetch beneficiaries"));
        }
      }
    }),

    deleteBeneficiary: builder.query<{ success: boolean; message?: string }, DeleteBeneficiaryRequest>({
      query: (request) => ({
        url: `/beneficiaries/${request.id}`,
        method: "DELETE"
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
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
  useLazyGetBeneficiaryDetailQuery,
  useLazyBeneficiarySearchFilterQuery,
  useLazyDeleteBeneficiaryQuery,
  useLazyUpdateBeneficiaryQuery,
  useLazyGetBeneficiariesQuery,
  useLazyCreateBeneficiariesQuery,
  useLazyGetBeneficiarytypesQuery,
  useLazyCreateBeneficiaryContactQuery,
  useLazyGetBeneficiaryKindQuery
} = BeneficiariesApi;
