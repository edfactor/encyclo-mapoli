import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { setAccountingYearData } from "reduxstore/slices/lookupsSlice";
import { RootState } from "reduxstore/store";
import {
  CalendarResponseDto,
  ProfitYearRequest
} from "reduxstore/types";
import { url } from "./api";

export const LookupsApi = createApi({
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
        if (localImpersonation) {
          headers.set("impersonation", localImpersonation);
        }
      }
      return headers;
    }
  }),
  reducerPath: "LookupsApi",
  endpoints: (builder) => ({
    getAccountingYear: builder.query<CalendarResponseDto, ProfitYearRequest>({
      query: (params) => ({
        url: "calendar/accounting-year",
        method: "GET",
        params: {
          profitYear: params.profitYear
        }
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setAccountingYearData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    })
  })
});

export const { useLazyGetAccountingYearQuery } = LookupsApi;
