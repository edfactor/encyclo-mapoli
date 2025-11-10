import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { prepareHeaders, url } from "./api";

export const SecurityApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/security/`,
    mode: "cors",
    credentials: "include",
    prepareHeaders
  }),
  reducerPath: "securityApi",
  // Disable caching to prevent sensitive data from persisting in browser
  keepUnusedDataFor: 0, // Remove data immediately when no longer in use
  refetchOnMountOrArgChange: true, // Always fetch fresh data
  endpoints: (builder) => ({
    getUserRoles: builder.query({
      query: () => ({
        url: "user-roles"
      })
    }),
    getUserPermissions: builder.query({
      query: () => ({
        url: "user-permissions"
      })
    }),
    getUsername: builder.query({
      query: () => ({
        url: "username"
      })
    })
  })
});

export const { useGetUserRolesQuery, useGetUserPermissionsQuery, useGetUsernameQuery } = SecurityApi;
