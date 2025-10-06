export interface ServiceErrorResponse {
  data: ServiceProblemDetails;
}

export interface ServiceProblemDetails {
  title: string;
  detail?: string;
  status: number;
  errors?: ServiceValidationErrors;
}

export interface ServiceValidationErrors {
  [field: string]: string[];
}

// Note: This is a generic error format for unhandled exceptions from the backend
// that can come from the common library. It is not currently used by the UI but
// is defined here for future use.

export interface UnhandledErrorResponse {
  data: UnhandledErrrorDetails;
}

export interface UnhandledErrrorDetails {
  type: string;
  title: string;
  status: number;
  instance: string;
  traceId: string;
  errors: UnhandledError[];
}

export interface UnhandledError {
  name: string;
  reason: string;
  code: string;
  severity: string;
}
