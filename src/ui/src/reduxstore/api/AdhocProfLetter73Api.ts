import { createApi } from "@reduxjs/toolkit/query/react";
import { PagedReportResponse } from "../../types/common/api";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export interface AdhocProfLetter73Request {
  profitYear: number;
}

export interface AdhocProfLetter73FormLetterRequest {
  profitYear: number;
  badgeNumbers: string[];
}

export interface AdhocProfLetter73Data {
  [key: string]: string | number | boolean | null | undefined; // Dynamic structure based on API response
}

export type AdhocProfLetter73Response = PagedReportResponse<AdhocProfLetter73Data>;

export const AdhocProfLetter73Api = createApi({
  baseQuery: baseQuery,
  reducerPath: "adhocProfLetter73Api",
  keepUnusedDataFor: 300,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getAdhocProfLetter73: builder.query<AdhocProfLetter73Response, AdhocProfLetter73Request>({
      query: (params) => {
        return {
          url: `/adhoc/prof-letter73?profitYear=${params.profitYear}`,
          method: "GET"
        };
      }
    }),
    downloadAdhocProfLetter73FormLetter: builder.query<Blob, AdhocProfLetter73FormLetterRequest>({
      query: (params) => ({
        url: "/adhoc/prof-letter73/download-form-letter",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          badgeNumbers: params.badgeNumbers
        },
        responseHandler: (response) => response.blob()
      })
    })
  })
});

export const { 
  useGetAdhocProfLetter73Query, 
  useLazyGetAdhocProfLetter73Query,
  useLazyDownloadAdhocProfLetter73FormLetterQuery
} = AdhocProfLetter73Api;
