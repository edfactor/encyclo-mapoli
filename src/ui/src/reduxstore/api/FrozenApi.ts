import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
    frozenStateResponse
} from "reduxstore/types";
import {
    setDuplicateSSNsData,   
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
        getDuplicateSSNs: builder.query<frozenStateResponse>({
            query: (params) => ({
                url: `yearend/duplicate-ssns`,
                method: "GET"                
            }),
            async onQueryStarted(arg, { dispatch, queryFulfilled }) {
                try {
                    const { data } = await queryFulfilled;
                    dispatch(setDuplicateSSNsData(data));
                } catch (err) {
                    console.log("Err: " + err);
                }
            }
        })
    })
});

export const {
    useLazygetDuplicateSSNQuery,
    
} = FrozenApi;
