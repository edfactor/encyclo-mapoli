import type { PagedReportResponse } from "../common/api";

export interface RecentlyTerminatedDetail {
  badgeNumber: number;
  fullName: string;
  firstName: string;
  lastName: string;
  middleInitial: string;
  ssn: string;
  terminationDate: string;
  terminationCodeId: string;
  address: string;
  address2: string;
  city: string;
  state: string;
  postalCode: string;
  isExecutive: boolean;
}

export interface RecentlyTerminatedResponse extends PagedReportResponse<RecentlyTerminatedDetail> {
  resultHash: string;
}
