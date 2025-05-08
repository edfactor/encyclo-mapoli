import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
  GetNavigationStatusRequestDto,
  GetNavigationStatusResponseDto,
  UpdateNavigationRequestDto,
  UpdateNavigationResponseDto,
} from "reduxstore/types";
import { url } from "./api";
import { setNavigationStatus, setNavigationStatusError } from "reduxstore/slices/NavigationStatusSlice";
import { Paged } from "smart-ui-library";
import { NavigationApi } from "./NavigationApi";

export const NavigationStatusApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/navigation`,
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
          url: ``,
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

export const { useGetNavigationStatusQuery, useLazyGetNavigationStatusQuery, useLazyUpdateNavigationStatusQuery, useUpdateNavigationStatusQuery } = NavigationStatusApi;
