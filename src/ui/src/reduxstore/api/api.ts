import type { BaseQueryFn, FetchArgs, FetchBaseQueryError, FetchBaseQueryMeta } from "@reduxjs/toolkit/query";
import { fetchBaseQuery } from "@reduxjs/toolkit/query/react";

export const url = process.env.VITE_REACT_APP_PS_API as string;

export const tagTypes = ["Get"];

interface SecurityState {
  token: string | null;
  impersonating: string[];
}

interface AppState {
  security: SecurityState;
}

export const prepareHeaders = (headers: Headers, context: { getState: () => unknown }) => {
  const state = context.getState() as AppState;
  const token = state.security.token;
  const impersonating = state.security.impersonating;

  // If we have a token set in state, let's assume that we should be passing it.
  if (token) {
    headers.set("authorization", `Bearer ${token}`);
  }

  if (impersonating && impersonating.length > 0) {
    headers.set("impersonation", impersonating.join(" | "));
  }

  return headers;
};

/* -------------------------------------------------------------------------
   Automatic x-demographic-data-source header copier
   ------------------------------------------------------------------------- */
export const createDataSourceAwareBaseQuery = (
  timeout?: number
): BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> => {
  const rawBaseQuery = fetchBaseQuery({
    baseUrl: `${url}/api/`,
    mode: "cors",
    timeout: timeout ?? 100000, // Default 100 seconds, allow override for long-running operations
    prepareHeaders: (headers, { getState }) => {
      const root = getState() as AppState;
      const token = root.security.token;
      const impersonating = root.security.impersonating;
      if (token) {
        headers.set("authorization", `Bearer ${token}`);
      }
      if (impersonating && impersonating.length > 0) {
        headers.set("impersonation", impersonating.join(" | "));
      }
      // Add cache-busting headers to prevent browser caching
      headers.set("Cache-Control", "no-cache, no-store, must-revalidate");
      headers.set("Pragma", "no-cache");
      headers.set("Expires", "0");
      return headers;
    }
  });

  return async (args, api, extra) => {
    const result = await rawBaseQuery(args, api, extra);
    if (result.data && typeof result.data === "object") {
      const hdr =
        (result.meta as FetchBaseQueryMeta | undefined)?.response?.headers?.get("x-demographic-data-source") ?? "Live";
      (result.data as Record<string, string>).dataSource = hdr;
    }
    return result;
  };
};
