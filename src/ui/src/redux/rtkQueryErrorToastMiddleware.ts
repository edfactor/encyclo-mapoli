/* eslint-disable @typescript-eslint/no-explicit-any */
import type { Middleware, MiddlewareAPI, PayloadAction } from "@reduxjs/toolkit";
import { isRejectedWithValue } from "@reduxjs/toolkit";
import { ToastServiceUtils } from "smart-ui-library";
import { ServiceErrorResponse } from "../types/errors/errors";
import { createValidationErrorsMessage } from "../utils/errorUtils";

interface MetaArg {
  endpointName: string;
  suppressAllToastErrors?: boolean;
  onlyNetworkToastErrors?: boolean;
}

/**
 * An RTK Query middleware that intercepts rejected actions and displays error messages using a toast service.
 * @param showErrors - A boolean flag indicating whether to show error messages.
 */

export const rtkQueryErrorToastMiddleware =
  (showErrors: boolean): Middleware =>
  (_api: MiddlewareAPI) =>
  (next) =>
  (action: unknown) => {
    if (showErrors && isRejectedWithValue(action)) {
      const payload = (action as PayloadAction<unknown>).payload as ServiceErrorResponse;
      const payloadData = payload.data;

      // These meta fields are added by RTK Query to the action meta and where we will find
      // properties to tell us how to manage errors
      const arg = (action as any).meta?.arg as MetaArg;
      const originalArg = (action as any).meta?.arg?.originalArgs || (action as any).meta?.arg;
      const queryMeta = (action as any).meta?.arg?.meta;

      // Useful for debugging

      /*
      console.log("RTK Error Middleware Debug:", {
        endpointName: arg?.endpointName,
        suppressAllToastErrors: arg?.suppressAllToastErrors,
        onlyNetworkToastErrors: arg?.onlyNetworkToastErrors,
        metaArg: (action as any).meta?.arg,
        originalArg,
        originalArgSuppressErrorToast: originalArg?.suppressAllToastErrors,
        originalArgOnlyNetworkToastErrors: originalArg?.onlyNetworkToastErrors,
        queryMeta,
        queryMetaSuppressErrorToast: queryMeta?.suppressAllToastErrors,
        queryMetaOnlyNetworkToastErrors: queryMeta?.onlyNetworkToastErrors
      });
      */

      if (arg?.suppressAllToastErrors || originalArg?.suppressAllToastErrors || queryMeta?.suppressAllToastErrors) {
        return next(action);
      }

      if (!payloadData) {
        // This is a timeout or network error, which we will show all the time
        const msgStr = `Service: "${arg?.endpointName?.toUpperCase()}" unavailable`;
        ToastServiceUtils.triggerError(msgStr);
        return next(action);
      }

      // If onlyNetworkToastErrors is set, skip showing validation-style errors
      if (!(arg?.onlyNetworkToastErrors || originalArg?.onlyNetworkToastErrors || queryMeta?.onlyNetworkToastErrors)) {
        const statusCode = Number(payloadData.status);
        console.log("RTK Error Middleware Status Code:", statusCode);

        if (statusCode && payloadData.errors) {
          if (statusCode == 400) {
            const validationErrors = createValidationErrorsMessage(payload);
            ToastServiceUtils.triggerError(validationErrors);
          } else if (statusCode === 401 || statusCode === 403) {
            // Authorization errors
            const msgStr = `Authorization unsuccessful for "${arg?.endpointName?.toUpperCase()}" endpoint`;
            ToastServiceUtils.triggerError(msgStr);
          }
        } else if (statusCode > 400) {
          // Other client errors
          const msgStr = `Authorization error for "${arg?.endpointName?.toUpperCase()}": • ${payloadData.detail}`;
          ToastServiceUtils.triggerError(msgStr);
        } else if (statusCode === 404) {
          // Not found errors
          const msgStr = `Resource not found for "${arg?.endpointName?.toUpperCase()}" endpoint`;
          ToastServiceUtils.triggerError(msgStr);
        } else if (statusCode == 400) {
          // Other client errors
          const msgStr = `Validation error for "${arg?.endpointName?.toUpperCase()}": • ${payloadData.detail}`;
          ToastServiceUtils.triggerError(msgStr);
        } else if (statusCode >= 500) {
          // Server errors
          const msgStr = `Server error for "${arg?.endpointName?.toUpperCase()}": • ${payloadData.detail}`;
          ToastServiceUtils.triggerError(msgStr);
        } else {
          // Fallback for timeout or other issues
          const msgStr = `Service call unsuccessful for "${arg?.endpointName?.toUpperCase()}" endpoint`;
          ToastServiceUtils.triggerError(msgStr);
        }
      }
    }

    return next(action);
  };
