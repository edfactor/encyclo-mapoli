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
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(setNavigation(data));
        } catch (err) {
          console.error("Failed to fetch navigation:", err);
          dispatch(setNavigationError("Failed to fetch military contributions"));
        }
      }
    })
  })
});

export const { useGetNavigationQuery, useLazyGetNavigationQuery } = NavigationApi;
