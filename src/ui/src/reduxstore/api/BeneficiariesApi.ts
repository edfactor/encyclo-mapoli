import { createApi } from "@reduxjs/toolkit/query/react";

import {
    BeneficiaryDto,
    BeneficiaryKindRequestDto,
    BeneficiaryKindResponseDto,
    BeneficiaryRequestDto,
    BeneficiaryTypesRequestDto,
    BeneficiaryTypesResponseDto,
    CreateBeneficiaryContactRequest,
    CreateBeneficiaryContactResponse,
    CreateBeneficiaryRequest,
    CreateBeneficiaryResponse
} from "reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";
import { Paged } from "smart-ui-library";
import { setBeneficiary, setBeneficiaryError } from "reduxstore/slices/beneficiarySlice";

const baseQuery = createDataSourceAwareBaseQuery();
export const BeneficiariesApi = createApi({
    baseQuery: baseQuery,
    reducerPath: "beneficiariesApi",
    endpoints: (builder) => ({
        getBeneficiaries: builder.query<Paged<BeneficiaryDto>, BeneficiaryRequestDto>({
            query: (request) => ({
                url: `/beneficiary`,
                method: "GET",
                params: request
            }),
            async onQueryStarted(arg, { dispatch, queryFulfilled }) {
                try {
                    const { data } = await queryFulfilled;
                    dispatch(setBeneficiary(data));
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
                    const { data } = await queryFulfilled;
                    console.log(data);
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
                    const { data } = await queryFulfilled;
                    console.log(data);
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
                    const { data } = await queryFulfilled;
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
                    const { data } = await queryFulfilled;
                } catch (err) {
                    console.error("Failed to create beneficiary contact", err);
                    dispatch(setBeneficiaryError("Failed to create beneficiary contact"));
                }
            }
        })
    })
});

export const { useLazyGetBeneficiariesQuery, useLazyCreateBeneficiariesQuery, useLazyGetBeneficiarytypesQuery, useLazyCreateBeneficiaryContactQuery, useLazyGetBeneficiaryKindQuery } = BeneficiariesApi;
