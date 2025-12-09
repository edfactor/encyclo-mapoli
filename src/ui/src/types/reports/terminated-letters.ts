import { PagedReportResponse, SortedPaginationRequestDto } from "../index";

export interface TerminatedLettersRequest {
  profitYear: number;
  beginningDate?: string;
  endingDate?: string;
  excludeZeroBalance?: boolean;
  badgeNumbers?: number[];
  pagination: SortedPaginationRequestDto;
}

export interface TerminatedLettersDetail {
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

export interface TerminatedLettersResponse extends PagedReportResponse<TerminatedLettersDetail> {}
