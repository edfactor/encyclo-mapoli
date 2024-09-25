import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/dist/query/react";
import { prepareHeaders, url } from "./api";

export const SecurityApi = createApi({
  baseQuery: fetchBaseQuery({
    baseUrl: `${url}/api/security/`,
    mode: "cors",
    credentials: "include",
    prepareHeaders
  }),

  reducerPath: "securityApi",

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
