import { createApi } from "@reduxjs/toolkit/query/react";

import { setNavigationStatus, setNavigationStatusError } from "../../reduxstore/slices/NavigationStatusSlice";
import {
  GetNavigationStatusRequestDto,
  GetNavigationStatusResponseDto,
  UpdateNavigationRequestDto,
  UpdateNavigationResponseDto
} from "../../reduxstore/types";
import { createDataSourceAwareBaseQuery } from "./api";
import { NavigationApi } from "./NavigationApi";

const baseQuery = createDataSourceAwareBaseQuery();

export const NavigationStatusApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "navigationStatusApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getNavigationStatus: builder.query<GetNavigationStatusResponseDto, GetNavigationStatusRequestDto>({
      query: (_request) => ({
        url: `/common/navigation/status`,
        method: "GET"
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
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
        url: `/common/navigation/status`,
        method: "PUT",
        body: request
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          if (data.isSuccessful) {
            dispatch(NavigationApi.endpoints.getNavigation.initiate({ navigationId: undefined }));
          }
        } catch (err) {
          console.error("Failed to fetch navigation status:", err);
          dispatch(setNavigationStatusError("Failed to fetch navigation status"));
        }
      }
    })
  })
});

export const {
  useGetNavigationStatusQuery,
  useLazyGetNavigationStatusQuery,
  useLazyUpdateNavigationStatusQuery,
  useUpdateNavigationStatusQuery
} = NavigationStatusApi;
