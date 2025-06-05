import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { createDataSourceAwareBaseQuery, prepareHeaders, url } from "./api";
import { setVersionInfo } from "reduxstore/slices/commonSlice";

export interface AppVersionInfo {
  buildNumber: string;
  gitHash: string;
  shortGitHash: string;
}

const baseQuery = createDataSourceAwareBaseQuery();

export const CommonApi = createApi({
  baseQuery: baseQuery,
  reducerPath: "commonApi",
  endpoints: (builder) => ({
    getAppVersion: builder.query<AppVersionInfo, void>({
      query: () => ({
        url: "/common/app-version-info"
      }),
      async onQueryStarted(val: void, { dispatch, queryFulfilled }) {
          const { data } = await queryFulfilled;
          dispatch(setVersionInfo(data));
      }
    })
  })
});

export const { useGetAppVersionQuery } = CommonApi;
