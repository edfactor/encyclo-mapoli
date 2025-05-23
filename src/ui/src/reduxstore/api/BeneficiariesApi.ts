import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
    BeneficiaryDto,
    BeneficiaryRequestDto,
    BeneficiaryResponseDto
} from "reduxstore/types";
import { url } from "./api";
import { Paged } from "smart-ui-library";
import { setBeneficiary, setBeneficiaryError } from "reduxstore/slices/beneficiarySlice";

export const BeneficiariesApi = createApi({
    baseQuery: fetchBaseQuery({
        baseUrl: `${url}/api/beneficiary`,
        prepareHeaders: (headers, { getState }) => {
            const token = (getState() as RootState).security.token;
            const impersonating = (getState() as RootState).security.impersonating;
            if (token) {
                headers.set("authorization", `Bearer ${token}`);
            }
            if (impersonating) {
                headers.set("impersonation", impersonating);
            } else {
                const localImpersonation = localStorage.getItem("impersonatingRole");
                if (localImpersonation) {
                    headers.set("impersonation", localImpersonation);
                }
            }
            return headers;
        }
    }),
    reducerPath: "beneficiariesApi",
    endpoints: (builder) => ({
        getBeneficiaries: builder.query<Paged<BeneficiaryDto>, BeneficiaryRequestDto>({
            query: (request) => ({
                url: ``,
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
        })
    })
});

export const { useGetBeneficiariesQuery, useLazyGetBeneficiariesQuery } = BeneficiariesApi;
