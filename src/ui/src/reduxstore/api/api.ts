import { RootState } from "../store";
import type { BaseQueryFn, FetchArgs, FetchBaseQueryError, FetchBaseQueryMeta } from "@reduxjs/toolkit/query";
import { fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export const url = process.env.VITE_REACT_APP_PS_API as string;

export const tagTypes = ["Get"];

export const prepareHeaders = (headers: any, context: any) => {
  const token = (context.getState() as RootState).security.token;

  // If we have a token set in state, let's assume that we should be passing it.
  if (token) {
    headers.set("authorization", `Bearer ${token}`);
  }

  return headers;
};

/* -------------------------------------------------------------------------
   Automatic x-demographic-data-source header copier
   ------------------------------------------------------------------------- */
export const createDataSourceAwareBaseQuery = (): BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> => {
  const rawBaseQuery = fetchBaseQuery({
    baseUrl: `${url}/api/`,
    mode: "cors",
    prepareHeaders: (headers, { getState }) => {
      const root = getState() as RootState;
      const token = root.security.token;
      const impersonating = root.security.impersonating;
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
  });

  return async (args, api, extra) => {
    const result = await rawBaseQuery(args, api, extra);
    if (result.data && typeof result.data === "object") {
      const hdr =
        (result.meta as FetchBaseQueryMeta | undefined)?.response?.headers?.get("x-demographic-data-source") ?? "Live";
      (result.data as Record<string, unknown>).dataSource = hdr;
    }
    return result;
  };
};
