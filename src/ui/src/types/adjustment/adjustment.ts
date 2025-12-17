export interface MergeProfitsDetailRequest {
  sourceSsn: number;
  destinationSsn: number;
}

export interface ReverseProfitDetailsRequest {
  ids: number[];
}

export interface ReverseProfitDetailsResponse {
  ids: number[];
}
