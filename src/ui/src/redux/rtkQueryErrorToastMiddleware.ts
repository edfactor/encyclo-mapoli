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
    if (
      showErrors &&
      isRejectedWithValue(action)
    ) {
      const payload = (action as PayloadAction<unknown>).payload as ErrorPayload;
      const payloadData = payload.data;
      const arg = (action as any).meta?.arg as MetaArg;

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
