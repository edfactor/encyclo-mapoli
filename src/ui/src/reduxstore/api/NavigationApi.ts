import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { RootState } from "reduxstore/store";
import {
  CreateMilitaryContributionRequest,
  MasterInquiryDetail,
  MilitaryContributionRequest,
  NavigationRequestDto,
  NavigationResponseDto,
  PagedReportResponse
} from "reduxstore/types";
import { url } from "./api";
import { setNavigation, setNavigationError } from "reduxstore/slices/navigationSlice";
import { Paged } from "smart-ui-library";

export const NavigationApi = createApi({
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
  reducerPath: "navigationApi",
  endpoints: (builder) => ({
    getNavigation: builder.query<NavigationResponseDto, NavigationRequestDto>({
      query: (request) => ({
        url: ``,
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
