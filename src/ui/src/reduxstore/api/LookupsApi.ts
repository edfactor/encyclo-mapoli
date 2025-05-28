import { createApi } from "@reduxjs/toolkit/query/react";

import { setAccountingYearData, setMissivesData } from "reduxstore/slices/lookupsSlice";
import {
  CalendarResponseDto,
  MissiveResponse,
  ProfitYearRequest
} from "reduxstore/types";
import { createDataSourceAwareBaseQuery, url } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();
export const LookupsApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "lookupsApi",
  endpoints: (builder) => ({
    getAccountingYear: builder.query<CalendarResponseDto, ProfitYearRequest>({
      query: (params) => ({
        url: "/lookup/calendar/accounting-year",
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
    }),
    getMissives: builder.query<MissiveResponse[],void>({
      query: (params) => ({
        url: "missives",
        method: "GET",
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMissivesData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    })
  })
});

export const { useLazyGetAccountingYearQuery, useLazyGetMissivesQuery } = LookupsApi;
