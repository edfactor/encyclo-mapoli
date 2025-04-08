import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { prepareHeaders, url } from "./api";
import { setVersionInfo } from "reduxstore/slices/commonSlice";

export interface AppVersionInfo {
  buildNumber: string;
  gitHash: string;
  shortGitHash: string;
}

export const CommonApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/common/`,
    mode: "cors",
    prepareHeaders
  }),

  reducerPath: "commonApi",

  endpoints: (builder) => ({
    getAppVersion: builder.query<AppVersionInfo, void>({
      query: () => ({
        url: "app-version-info"
      }),
      async onQueryStarted(val: void, { dispatch, queryFulfilled }) {
          const { data } = await queryFulfilled;
          dispatch(setVersionInfo(data));
      }
    })
  })
});

export const { useGetAppVersionQuery } = CommonApi;
