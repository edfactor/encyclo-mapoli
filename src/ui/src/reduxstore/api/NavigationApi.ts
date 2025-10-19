import { createApi } from "@reduxjs/toolkit/query/react";

import { setNavigation, setNavigationError } from "../../reduxstore/slices/navigationSlice";
import { NavigationRequestDto, NavigationResponseDto } from "../../reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const NavigationApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "navigationApi",
  endpoints: (builder) => ({
    getNavigation: builder.query<NavigationResponseDto, NavigationRequestDto>({
      query: (_request) => ({
        url: `/navigation`,
        method: "GET"
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          // Normalize navigation items to ensure `isNavigable` is present (default true)
          const normalize = (items) => {
            if (!items) return items;
            return items.map((it) => ({
              ...it,
              isNavigable: it.isNavigable === undefined ? true : it.isNavigable,
              items: normalize(it.items),
              prerequisiteNavigations: normalize(it.prerequisiteNavigations)
            }));
          };

          const normalized = data ? { ...data, navigation: normalize(data.navigation) } : data;
          dispatch(setNavigation(normalized));
        } catch (err) {
          console.error("Failed to fetch navigation:", err);
          // More descriptive error message
          const errorMessage =
            err.error?.status === 401
              ? "Authentication error - please log in again"
              : "Failed to fetch navigation data";

          dispatch(setNavigationError(errorMessage));
        }
      }
    })
  })
});

export const { useGetNavigationQuery, useLazyGetNavigationQuery } = NavigationApi;
