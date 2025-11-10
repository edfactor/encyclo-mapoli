import { createApi } from "@reduxjs/toolkit/query/react";
import { setVersionInfo } from "reduxstore/slices/commonSlice";
import { createDataSourceAwareBaseQuery } from "./api";

export interface AppVersionInfo {
  buildNumber: string;
  gitHash: string;
  shortGitHash: string;
}

const baseQuery = createDataSourceAwareBaseQuery();

export const CommonApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "commonApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getAppVersion: builder.query<AppVersionInfo, void>({
      query: () => ({
        url: "/common/app-version-info"
      }),
      async onQueryStarted(_val: void, { dispatch, queryFulfilled }) {
        const { data } = await queryFulfilled;
        dispatch(setVersionInfo(data));
      }
    })
  })
});

export const { useGetAppVersionQuery } = CommonApi;
