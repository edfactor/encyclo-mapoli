import { createApi } from "@reduxjs/toolkit/query/react";

import {
    BeneficiaryDto,
    BeneficiaryRequestDto,
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
        })
    })
});

export const { useLazyGetBeneficiariesQuery } = BeneficiariesApi;
