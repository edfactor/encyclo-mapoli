import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { prepareHeaders, url } from "./api";
import { Health } from "../healthTypes";
import { setHealthInfo } from "../slices/appSupportSlice";

export const AppSupportApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/`,
    mode: "cors",
    prepareHeaders
  }),
  reducerPath: "appSupportApi",
  endpoints: (builder) => ({
    getHealth: builder.query<Health, void>({
      query: () => ({
        url: "health"
      }),
      async onQueryStarted(_val: void, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setHealthInfo(data));
        } catch (error) {
          console.error(error);
        }
      }
    })
  })
});

export const { useGetHealthQuery, useLazyGetHealthQuery } = AppSupportApi;
