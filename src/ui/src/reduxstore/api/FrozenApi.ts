import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import { FrozenStateResponse } from "reduxstore/types";
import {
    setFrozenStateResponse,
    setFrozenStateCollectionResponse
} from "reduxstore/slices/frozenSlice";
import { url } from "./api";

export const FrozenApi = createApi({
    baseQuery: fetchBaseQuery({
        baseUrl: `${url}/api/`,
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
                !!localImpersonation && headers.set("impersonation", localImpersonation);
            }
            return headers;
        }
    }),
    reducerPath: "frozenApi",
    endpoints: (builder) => ({
        getFrozenStateResponse: builder.query<FrozenStateResponse, void>({
            query: () => ({
                url: `demographics/frozen/active`,
                method: "GET",
            }),
            async onQueryStarted(arg, { dispatch, queryFulfilled }) {
                try {
                    const { data } = await queryFulfilled;
                    dispatch(setFrozenStateResponse(data));
                } catch (err) {
                    console.error("Failed to fetch frozen state:", err);
                    dispatch(setFrozenStateResponse(null)); // Handle API errors
                }
            }
        }),
        getHistoricalFrozenStateResponse: builder.query<FrozenStateResponse[], void>({
            query: () => ({
                url: `demographics/frozen`,
                method: "GET",
            }),
            async onQueryStarted(arg, { dispatch, queryFulfilled }) {
                try {
                    const { data } = await queryFulfilled;
                    dispatch(setFrozenStateCollectionResponse(data));
                } catch (err) {
                    console.error("Failed to fetch frozen state collection:", err);
                    dispatch(setFrozenStateCollectionResponse(null)); // Handle API errors
                }
            }
        })
    })
});

export const {
    useLazyGetFrozenStateResponseQuery,
    useLazyGetHistoricalFrozenStateResponseQuery    
} = FrozenApi;