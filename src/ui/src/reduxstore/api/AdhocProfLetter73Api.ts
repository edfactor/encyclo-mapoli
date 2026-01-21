import { createApi } from "@reduxjs/toolkit/query/react";
import { RobustlyPaged } from "../../types/common/api";
import { createDataSourceAwareBaseQuery } from "./api";

const baseQuery = createDataSourceAwareBaseQuery();

export interface AdhocProfLetter73Request {
  profitYear: number;
  DeMinimusValue?: number | null;
  skip?: number;
  take?: number;
  sortBy?: string;
  isSortDescending?: boolean;
}

export interface AdhocProfLetter73FormLetterRequest {
  profitYear: number;
  badgeNumbers: string[];
  isXerox?: boolean;
}

export interface AdhocProfLetter73Data {
  [key: string]: string | number | boolean | null | undefined; // Dynamic structure based on API response
}

export type AdhocProfLetter73Response = RobustlyPaged<AdhocProfLetter73Data>;

export const AdhocProfLetter73Api = createApi({
  baseQuery: baseQuery,
  reducerPath: "adhocProfLetter73Api",
  keepUnusedDataFor: 300,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getAdhocProfLetter73: builder.query<AdhocProfLetter73Response, AdhocProfLetter73Request>({
      query: (params) => {
        const queryParams = new URLSearchParams({
          profitYear: params.profitYear.toString()
        });

        if (params.DeMinimusValue !== undefined && params.DeMinimusValue !== null) {
          queryParams.append("DeMinimusValue", params.DeMinimusValue.toString());
        }
        if (params.skip !== undefined) {
          queryParams.append("skip", params.skip.toString());
        }
        if (params.take !== undefined) {
          queryParams.append("take", params.take.toString());
        }
        if (params.sortBy) {
          queryParams.append("sortBy", params.sortBy);
        }
        if (params.isSortDescending !== undefined) {
          queryParams.append("isSortDescending", params.isSortDescending.toString());
        }

        return {
          url: `/ad-hoc/prof-letter73?${queryParams.toString()}`,
          method: "GET"
        };
      }
    }),
    downloadAdhocProfLetter73FormLetter: builder.query<Blob, AdhocProfLetter73FormLetterRequest>({
      query: (params) => ({
        url: "/ad-hoc/prof-letter73/download-form-letter",
        method: "GET",
        params: {
          profitYear: params.profitYear,
          badgeNumbers: params.badgeNumbers,
          isXerox: params.isXerox
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
