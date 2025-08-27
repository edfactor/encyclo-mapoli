import type { Middleware, MiddlewareAPI, PayloadAction } from "@reduxjs/toolkit";
import { isRejectedWithValue } from "@reduxjs/toolkit";
import { ToastServiceUtils } from "smart-ui-library";

interface ErrorPayload {
  data?: {
    statusCode?: number;
    StatusCode?: number;
    message?: string;
    Message?: string;
    errors?: {
      [field: string]: string[];
    };
    Errors?: string[];
  };
}

interface MetaArg {
  endpointName: string;
  suppressAllToastErrors?: boolean;
  onlyNetworkToastErrors?: boolean;
}

/**
 * Utility function to extract error details from RTK Query error objects
 * @param error - The RTK Query error object
 * @returns Object containing statusCode, message, and formatted error details
 */
export const extractErrorDetails = (error: ErrorPayload | undefined) => {
  if (!error || !error.data) {
    return {
      statusCode: 0,
      message: "Request failed or timed out",
      formattedMessage: "Request failed or timed out"
    };
  }

  const { data } = error;
  const statusCode = data.statusCode || data.StatusCode || 0;
  const message = data.message || data.Message || "Unknown error";

  let formattedMessage = `${statusCode} : ${message}`;

  // Handle field-level validation errors
  if (data.errors) {
    let fieldErrorsStr = "";

    Object.entries(data.errors).forEach(([field, messages]) => {
      fieldErrorsStr += `\n${field}:`;
      (messages as string[]).forEach((message) => {
        fieldErrorsStr += `\n  • ${message}`;
      });
    });

    if (fieldErrorsStr) {
      formattedMessage += fieldErrorsStr;
    }
  } else if (data.Errors && data.Errors.length > 0) {
    formattedMessage += "\nErrors:";
    data.Errors.forEach((error: string) => {
      formattedMessage += `\n  • ${error}`;
    });
  }

  return {
    statusCode,
    message,
    formattedMessage,
    fieldErrors: data.errors,
    errors: data.Errors
  };
};

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
      const payload = (action as PayloadAction<unknown>).payload as ErrorPayload;
      const payloadData = payload.data;
      const arg = (action as any).meta?.arg as MetaArg;
      const originalArg = (action as any).meta?.arg?.originalArgs || (action as any).meta?.arg;
      const queryMeta = (action as any).meta?.arg?.meta;

      // Useful for debugging
      /*
      console.log('RTK Error Middleware Debug:', {
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
        //console.log('Suppressing error toast for:', arg?.endpointName);
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
        // Handle both camelCase and PascalCase properties for backward compatibility
        const statusCode = payloadData.statusCode || payloadData.StatusCode;
        const message = payloadData.message || payloadData.Message;

        if (statusCode && statusCode >= 400) {
          // Create the base error message
          let msgStr = `${statusCode} : ${message}`;

          // Handle field-level validation errors
          if (payloadData.errors) {
            // Format field errors with each field on its own line and indented messages
            let fieldErrorsStr = "";

            Object.entries(payloadData.errors).forEach(([field, messages]) => {
              // Add field name
              fieldErrorsStr += `\n${field}:`;

              // Add indented messages
              messages.forEach((message) => {
                fieldErrorsStr += `\n  • ${message}`;
              });
            });

            if (fieldErrorsStr) {
              msgStr += fieldErrorsStr;
            }
          } else if (payloadData.Errors && payloadData.Errors.length > 0) {
            // Handle legacy error format with a similar format
            msgStr += "\nErrors:";
            payloadData.Errors.forEach((error) => {
              msgStr += `\n  • ${error}`;
            });
          }

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
