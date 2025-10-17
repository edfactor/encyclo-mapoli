import { createApi } from "@reduxjs/toolkit/query/react";

import { setAccountingYearData, setMissivesData, setStateTaxData } from "reduxstore/slices/lookupsSlice";
import {
  CalendarResponseDto,
  MissiveResponse,
  ProfitYearRequest,
  YearRangeRequest,
  StateTaxLookupResponse
} from "reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";

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
    getMissives: builder.query<MissiveResponse[], void>({
      query: () => ({
        url: "/lookup/missives",
        method: "GET"
      }),
      transformResponse: (response: { items: MissiveResponse[]; count: number }) => {
        return response.items;
      },
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setMissivesData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    }),
    getAccountingRange: builder.query<CalendarResponseDto, YearRangeRequest>({
      query: (params) => ({
        url: "/lookup/calendar/accounting-range",
        method: "GET",
        params: {
          beginProfitYear: params.beginProfitYear,
          endProfitYear: params.endProfitYear
        }
      })
    }),
    getDuplicateSsnExists: builder.query<boolean, void>({
      query: () => ({
        url: "/lookup/duplicate-ssns/exists",
        method: "GET"
      })
    }),
    getStateTax: builder.query<StateTaxLookupResponse, string>({
      query: (state) => ({
        url: `/lookup/state-taxes/${state}`,
        method: "GET"
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setStateTaxData(data));
        } catch (err) {
          console.log("Err: " + err);
        }
      }
    })
  })
});

export const {
  useLazyGetAccountingYearQuery,
  useLazyGetMissivesQuery,
  useLazyGetAccountingRangeQuery,
  useLazyGetDuplicateSsnExistsQuery,
  useLazyGetStateTaxQuery
} = LookupsApi;
