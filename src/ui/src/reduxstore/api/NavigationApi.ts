import { createApi } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
  NavigationRequestDto,
  NavigationResponseDto,  
} from "reduxstore/types";
import { createDataSourceAwareBaseQuery, url } from "./api";
import { setNavigation, setNavigationError } from "reduxstore/slices/navigationSlice";

const baseQuery = createDataSourceAwareBaseQuery();

export const NavigationApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "navigationApi",
  endpoints: (builder) => ({
    getNavigation: builder.query<NavigationResponseDto, NavigationRequestDto>({
      query: (request) => ({
        url: `/navigation`,
        method: "GET"
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled, getState }) {
        try {
          // Check token before attempting the query
          const state = getState() as RootState;
          const token = state.security.token;
          const { data } = await queryFulfilled;
          console.log("Navigation data successfully fetched");
          dispatch(setNavigation(data));
          if (!token) {
            console.warn("Navigation API called without a valid token");
            dispatch(setNavigationError("Authentication token missing"));
            return;
          }
          
          
        } catch (err) {
          console.error("Failed to fetch navigation:", err);
          // More descriptive error message
          const errorMessage = err.error?.status === 401 
            ? "Authentication error - please log in again" 
            : "Failed to fetch navigation data";
          
          dispatch(setNavigationError(errorMessage));
        }
      }
    })
  })
});

export const { useGetNavigationQuery, useLazyGetNavigationQuery } = NavigationApi;
