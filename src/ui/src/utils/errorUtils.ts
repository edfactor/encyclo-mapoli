import { ServiceErrorResponse } from "../types/errors/errors";

export const createValidationErrorsMessage = (message: ServiceErrorResponse): string => {
  const { title, detail, errors } = message.data;

  let msgStr = `${message}:\nErrors:`;

  // Handle field-level validation errors
  if (errors) {
    // Format field errors with each field on its own line and indented messages
    let fieldErrorsStr = "";

    Object.entries(errors).forEach(([field, messages]) => {
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
  } else if (detail) {
    // Not sure why this would be the case, but detail is better than nothing
    msgStr += `\n  • ${detail}`;
  }

  return `${title}: ${msgStr}`;
};
