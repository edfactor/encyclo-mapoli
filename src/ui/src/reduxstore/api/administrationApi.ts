import { createApi } from "@reduxjs/toolkit/query/react";

import {
    BankAccountDto,
    BankDto,
    CreateBankAccountRequest,
    CreateBankRequest,
    UpdateBankAccountRequest,
    UpdateBankRequest
} from "../../types/administration/banks";
import {
    CommentTypeDto,
    CreateCommentTypeRequest,
    GetMissingAnnuityYearsRequest,
    MissingAnnuityYearsResponse,
    RmdFactorDto,
    UpdateCommentTypeRequest,
    UpdateRmdFactorRequest
} from "../types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const AdministrationApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "administrationApi",
  tagTypes: ["CommentTypes", "RmdFactors", "Banks", "BankAccounts"],
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getMissingAnnuityYears: builder.query<MissingAnnuityYearsResponse, GetMissingAnnuityYearsRequest | void>({
      query: (request) => {
        const startYear = request?.startYear;
        const endYear = request?.endYear;
        const suppressAllToastErrors = request?.suppressAllToastErrors;
        const onlyNetworkToastErrors = request?.onlyNetworkToastErrors;

        const searchParams = new URLSearchParams();
        if (startYear !== undefined) searchParams.set("StartYear", String(startYear));
        if (endYear !== undefined) searchParams.set("EndYear", String(endYear));

        const queryString = searchParams.toString();
        const url = queryString
          ? `administration/annuity-rates/missing-years?${queryString}`
          : "administration/annuity-rates/missing-years";

        return {
          url,
          method: "GET",
          meta: { suppressAllToastErrors, onlyNetworkToastErrors }
        };
      }
    }),
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
        url: "/administration/rmds-factors",
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
        url: "/administration/rmds-factors",
        method: "PUT",
        body
      }),
      invalidatesTags: ["RmdFactors"]
    }),
    getAllBanks: builder.query<BankDto[], void>({
      query: () => ({
        url: "administration/banks",
        method: "GET"
      }),
      providesTags: ["Banks"]
    }),
    getBankById: builder.query<BankDto, number>({
      query: (id) => ({
        url: `administration,
  useGetAllBanksQuery,
  useGetBankByIdQuery,
  useCreateBankMutation,
  useUpdateBankMutation,
  useDisableBankMutation,
  useGetBankAccountsQuery,
  useLazyGetBankAccountsQuery,
  useGetBankAccountByIdQuery,
  useCreateBankAccountMutation,
  useUpdateBankAccountMutation,
  useSetPrimaryBankAccountMutation,
  useDisableBankAccountMutation/banks/${id}`,
        method: "GET"
      }),
      providesTags: (_result, _error, id) => [{ type: "Banks", id }]
    }),
    createBank: builder.mutation<BankDto, CreateBankRequest>({
      query: (request) => ({
        url: "administration/banks",
        method: "POST",
        body: request
      }),
      invalidatesTags: ["Banks"]
    }),
    updateBank: builder.mutation<BankDto, UpdateBankRequest>({
      query: (request) => ({
        url: "administration/banks",
        method: "PUT",
        body: request
      }),
      invalidatesTags: (_result, _error, arg) => [{ type: "Banks", id: arg.id }, "Banks"]
    }),
    disableBank: builder.mutation<boolean, number>({
      query: (id) => ({
        url: `administration/banks/${id}`,
        method: "DELETE"
      }),
      invalidatesTags: (_result, _error, id) => [{ type: "Banks", id }, "Banks"]
    }),
    getBankAccounts: builder.query<BankAccountDto[], number>({
      query: (bankId) => ({
        url: `administration/banks/${bankId}/accounts`,
        method: "GET"
      }),
      providesTags: (_result, _error, bankId) => [{ type: "BankAccounts", id: bankId }]
    }),
    getBankAccountById: builder.query<BankAccountDto, number>({
      query: (id) => ({
        url: `administration/bank-accounts/${id}`,
        method: "GET"
      }),
      providesTags: (_result, _error, id) => [{ type: "BankAccounts", id }]
    }),
    createBankAccount: builder.mutation<BankAccountDto, CreateBankAccountRequest>({
      query: (request) => ({
        url: "administration/bank-accounts",
        method: "POST",
        body: request
      }),
      invalidatesTags: (_result, _error, arg) => [{ type: "BankAccounts", id: arg.bankId }, "BankAccounts"]
    }),
    updateBankAccount: builder.mutation<BankAccountDto, UpdateBankAccountRequest>({
      query: (request) => ({
        url: "administration/bank-accounts",
        method: "PUT",
        body: request
      }),
      invalidatesTags: (_result, _error, arg) => [
        { type: "BankAccounts", id: arg.id },
        { type: "BankAccounts", id: arg.bankId },
        "BankAccounts"
      ]
    }),
    setPrimaryBankAccount: builder.mutation<boolean, number>({
      query: (id) => ({
        url: `administration/bank-accounts/${id}/set-primary`,
        method: "PATCH"
      }),
      invalidatesTags: ["BankAccounts"]
    }),
    disableBankAccount: builder.mutation<boolean, number>({
      query: (id) => ({
        url: `administration/bank-accounts/${id}`,
        method: "DELETE"
      }),
      invalidatesTags: (_result, _error, id) => [{ type: "BankAccounts", id }, "BankAccounts"]
    })
  })
});

export const {
  useGetMissingAnnuityYearsQuery,
  useGetCommentTypesQuery,
  useCreateCommentTypeMutation,
  useUpdateCommentTypeMutation,
  useGetRmdFactorsQuery,
  useUpdateRmdFactorMutation,
  useGetAllBanksQuery,
  useGetBankByIdQuery,
  useCreateBankMutation,
  useUpdateBankMutation,
  useDisableBankMutation,
  useGetBankAccountsQuery,
  useLazyGetBankAccountsQuery,
  useGetBankAccountByIdQuery,
  useCreateBankAccountMutation,
  useUpdateBankAccountMutation,
  useSetPrimaryBankAccountMutation,
  useDisableBankAccountMutation
} = AdministrationApi;
