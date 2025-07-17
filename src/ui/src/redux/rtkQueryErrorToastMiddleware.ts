import type { Middleware, MiddlewareAPI, PayloadAction } from "@reduxjs/toolkit";
import { isRejectedWithValue } from "@reduxjs/toolkit";
import { ToastServiceUtils } from "smart-ui-library";

interface ErrorPayload {
  data?: {
    statusCode?: number;
    StatusCode?: number; // Keep for backward compatibility
    message?: string;
    Message?: string; // Keep for backward compatibility
    errors?: {
      [field: string]: string[];
    };
    Errors?: string[]; // Keep for backward compatibility
  };
}

interface MetaArg {
  endpointName: string;
  suppressErrorToast?: boolean;
}

/**
 * Utility function to extract error details from RTK Query error objects
 * @param error - The RTK Query error object
 * @returns Object containing statusCode, message, and formatted error details
 */
export const extractErrorDetails = (error: any) => {
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
    if (
      showErrors &&
      isRejectedWithValue(action)
    ) {
      const payload = (action as PayloadAction<unknown>).payload as ErrorPayload;
      const payloadData = payload.data;
      const arg = (action as any).meta?.arg as MetaArg;
      
      // Skip toast if caller wants to handle errors manually
      if (arg?.suppressErrorToast) {
        return next(action);
      }

      if (!payloadData) {
        const msgStr = `Request timed out for "${arg?.endpointName?.toUpperCase()}" endpoint`;
        ToastServiceUtils.triggerError(msgStr);
        return next(action);
      }

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
        const msgStr = `Request timed out for "${arg?.endpointName?.toUpperCase()}" endpoint`;
        ToastServiceUtils.triggerError(msgStr);
      }
    }
    return next(action);
  };
