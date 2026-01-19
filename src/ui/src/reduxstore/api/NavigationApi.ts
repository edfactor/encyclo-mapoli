import { createApi } from "@reduxjs/toolkit/query/react";

import { setNavigation, setNavigationError } from "../../reduxstore/slices/navigationSlice";
import { NavigationDto, NavigationRequestDto, NavigationResponseDto } from "../../reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export const NavigationApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "navigationApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getNavigation: builder.query<NavigationResponseDto, NavigationRequestDto>({
      query: (_request) => ({
        url: `/common/navigation`,
        method: "GET"
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          // Normalize navigation items to ensure `isNavigable` is present (default true)
          const normalize = (items: NavigationDto[] | undefined): NavigationDto[] | undefined => {
            if (!items) return items;
            return items.map((it) => ({
              ...it,
              isNavigable: it.isNavigable === undefined ? true : it.isNavigable,
              items: normalize(it.items),
              prerequisiteNavigations: normalize(it.prerequisiteNavigations)
            })) as NavigationDto[];
          };

          const normalized: NavigationResponseDto | undefined = data
            ? { ...data, navigation: normalize(data.navigation) ?? [] }
            : data;
          dispatch(setNavigation(normalized ?? null));
        } catch (err) {
          console.error("Failed to fetch navigation:", err);
          // More descriptive error message
          const errorMessage =
            err &&
            typeof err === "object" &&
            "error" in err &&
            (err as Record<string, unknown>).error &&
            typeof (err as Record<string, unknown>).error === "object" &&
            (err as Record<string, Record<string, unknown>>).error.status === 401
              ? "Authentication error - please log in again"
              : "Failed to fetch navigation data";

          dispatch(setNavigationError(errorMessage));
        }
      }
    })
  })
});

export const { useGetNavigationQuery, useLazyGetNavigationQuery } = NavigationApi;
