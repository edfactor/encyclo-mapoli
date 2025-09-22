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
