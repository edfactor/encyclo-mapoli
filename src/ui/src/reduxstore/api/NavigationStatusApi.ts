import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import {
  GetNavigationStatusRequestDto,
  GetNavigationStatusResponseDto,
  UpdateNavigationRequestDto,
  UpdateNavigationResponseDto,
} from "reduxstore/types";
import { createDataSourceAwareBaseQuery, url } from "./api";
import { setNavigationStatus, setNavigationStatusError } from "reduxstore/slices/NavigationStatusSlice";
import { NavigationApi } from "./NavigationApi";

const baseQuery = createDataSourceAwareBaseQuery();

export const NavigationStatusApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "navigationStatusApi",
  endpoints: (builder) => ({
    getNavigationStatus: builder.query<GetNavigationStatusResponseDto, GetNavigationStatusRequestDto>({
      query: (request) => ({
        url: `status`,
        method: "GET"
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setNavigationStatus(data));
        } catch (err) {
          console.error("Failed to fetch navigation status:", err);
          dispatch(setNavigationStatusError("Failed to fetch navigation status"));
        }
      }
    }),
    updateNavigationStatus: builder.query<UpdateNavigationResponseDto, UpdateNavigationRequestDto>({
        query: (request) => ({
          url: `/navigation`,
          method: "PUT",
          body: request
        }),
        async onQueryStarted(arg, { dispatch, queryFulfilled }) {
          try {
            const { data } = await queryFulfilled;
            if(data.isSuccessful)
            {
                dispatch(
                    NavigationApi.endpoints.getNavigation.initiate({navigationId: undefined})
                );
            }
          } catch (err) {
            console.error("Failed to fetch navigation status:", err);
            dispatch(setNavigationStatusError("Failed to fetch navigation status"));
          }
        }
      })
  })
});

export const { useGetNavigationStatusQuery, useLazyUpdateNavigationStatusQuery, useUpdateNavigationStatusQuery } = NavigationStatusApi;
